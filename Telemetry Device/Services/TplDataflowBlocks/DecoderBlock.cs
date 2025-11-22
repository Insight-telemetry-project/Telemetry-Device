using Microsoft.Extensions.Options;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Icd;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Mongo;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services.Mongo;

public class DecoderBlock : IDecoderBlock
{
    private readonly IBitOperations _bitOperations;
    private readonly IIcdProvider _icdProvider;
    private readonly List<IcdField> _icd;
    private readonly TransformBlock<PacketData, DecodedFieldsPacket> _transformBlock;
    private readonly FlightHeaderSettings _flightHeader;
    private readonly FlightTelemetryMongoProxy _telemetryMongo;

    public DecoderBlock(IBitOperations bitOperations, IIcdProvider icdProvider, IOptions<FlightHeaderSettings> flightHeaderOptions, FlightTelemetryMongoProxy telemetryMongo)
    {
        _bitOperations = bitOperations;
        _icdProvider = icdProvider;
        _icd = _icdProvider.LoadIcd();
        _flightHeader = flightHeaderOptions.Value;
        _telemetryMongo = telemetryMongo;

        _transformBlock = new TransformBlock<PacketData, DecodedFieldsPacket>(
            ProcessPacketAsync,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                EnsureOrdered = false
            });
    }

    public ITargetBlock<PacketData> Input => _transformBlock;
    public ISourceBlock<DecodedFieldsPacket> Output => _transformBlock;

    public double DecodeFieldValue(PacketData packet, IcdField icdField)
    {
        int absoluteOffset = ConstantPackets.HEADER_BIT_OFFSET + icdField.BitOffset;
        ulong rawValue = _bitOperations.ReadBits(packet.Payload, absoluteOffset, icdField.SizeBits);
        return rawValue * icdField.Scale + icdField.Min;
    }

    private async Task<DecodedFieldsPacket> ProcessPacketAsync(PacketData packet)
    {
        System.Diagnostics.Debug.WriteLine($"[DECODER] {packet.Checksum}");

        Dictionary<string, int> databaseFields = new Dictionary<string, int>();
        Dictionary<string, double> streamingFields = new Dictionary<string, double>();

        foreach (IcdField field in _icd)
        {
            double value = DecodeFieldValue(packet, field);
            AssignField(field, value, databaseFields, streamingFields);
        }


        int masterIndex = databaseFields[ConstantPackets.FLIGHT_ID];

        if (!_telemetryMongo.ExistingMasterIndexes.Contains(masterIndex))
        {
            await _telemetryMongo.StoreFlightDataAsync(databaseFields);
            _telemetryMongo.existingMasterIndexes.Add(masterIndex);
        }


        return new DecodedFieldsPacket(streamingFields);
    }

    private void AssignField(IcdField field, double value, Dictionary<string, int> databaseFields, Dictionary<string, double> streamingFields)
    {
        string name = field.Name;

        if (name.Equals(ConstantPackets.FLIGHT_ID))
        {
            streamingFields[name] = value;
            databaseFields[name] = (int)value;
            return;
        }

        if (_flightHeader.Contains(name))
        {
            databaseFields[name] = (int)value;
            return;
        }

        streamingFields[name] = value;
    }
}
