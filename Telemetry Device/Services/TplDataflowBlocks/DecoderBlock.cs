using Microsoft.Extensions.Options;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using System.Collections.Generic;
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
    private readonly TelemetryMongo _telemetryMongo;
    public DecoderBlock(IBitOperations bitOperations, IIcdProvider icdProvider, IOptions<FlightHeaderSettings> flightHeaderOptions, TelemetryMongo telemetryMongo)
    {
        _bitOperations = bitOperations;
        _icdProvider = icdProvider;
        _icd = _icdProvider.LoadIcd();
        _flightHeader = flightHeaderOptions.Value;
        _telemetryMongo = telemetryMongo;

        //_transformBlock = new TransformBlock<PacketData, DecodedFieldsPacket>(packet =>
        //{
        //    Dictionary<string, double> fields = _icd
        //        .AsParallel()
        //        .ToDictionary(
        //            IcdField => IcdField.Name,
        //            icdField => DecodeFieldValue(packet, icdField)
        //        );
        //    return new DecodedFieldsPacket(fields);
        //});

        _transformBlock = new TransformBlock<PacketData, DecodedFieldsPacket>(async packet =>
        {
            Dictionary<string, int> mongoFields = new Dictionary<string, int>();
            Dictionary<string, double> kafkaFields = new Dictionary<string, double>();

            foreach (IcdField field in _icd)
            {
                double value = DecodeFieldValue(packet, field);

                if (field.Name.Equals(ConstantPackets.FLIGHT_ID))
                {
                    kafkaFields[field.Name] = value;
                    mongoFields[field.Name] = (int)value;
                }
                else if (_flightHeader.MongoFields.Contains(field.Name))
                {
                    mongoFields[field.Name] = (int)value;
                }
                else
                {
                    kafkaFields[field.Name] = value;
                }
            }
            await _telemetryMongo.SaveTelemetryAsync(mongoFields);
            return new DecodedFieldsPacket(kafkaFields);
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
}
