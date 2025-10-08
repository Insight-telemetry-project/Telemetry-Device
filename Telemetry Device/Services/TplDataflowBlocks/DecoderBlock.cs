using SendRecieveUDP.Model.Interfaces.BitManipulation;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Interface.Icd;

public class DecoderBlock : IDecoderBlock
{
    private readonly IBitOperations _bitOperations;
    private readonly IIcdProvider _icdProvider;
    private readonly List<IcdField> _icd;
    private readonly TransformBlock<PacketData, DictionaryPacket> _transformBlock;

    public DecoderBlock(IBitOperations bitOperations, IIcdProvider icdProvider)
    {
        _bitOperations = bitOperations;
        _icdProvider = icdProvider;
        _icd = _icdProvider.LoadIcd();
        _transformBlock = new TransformBlock<PacketData, DictionaryPacket>(packet =>
        {
            Dictionary<string, double> fields = _icd
                .AsParallel()
                .ToDictionary(
                    IcdField => IcdField.Name,
                    icdField => DecodeFieldValue(packet, icdField)
                );
            return new DictionaryPacket(fields);
        });
    }
    public ITargetBlock<PacketData> Input => _transformBlock;
    public ISourceBlock<DictionaryPacket> Output => _transformBlock;

    public double DecodeFieldValue(PacketData packet, IcdField icdField)
    {
        int absoluteOffset = ConstantPackets.HEADER_BIT_OFFSET + icdField.BitOffset;
        ulong rawValue = _bitOperations.ReadBits(packet.Payload, absoluteOffset, icdField.SizeBits);
        return rawValue * icdField.Scale + icdField.Min;
    }
}
