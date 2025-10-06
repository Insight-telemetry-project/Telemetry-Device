using SendRecieveUDP.Model.Interfaces.BitManipulation;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Interface.Icd;

public class DecoderBlock : IDecoderBlock
{
    private readonly IBitEncoder _bitEncoder;
    private readonly IIcdProvider _icdProvider;
    private readonly List<IcdField> _icd;

    public DecoderBlock(IBitEncoder bitEncoder, IIcdProvider icdProvider)
    {
        _bitEncoder = bitEncoder;
        _icdProvider = icdProvider;
        _icd = _icdProvider.LoadIcd();
    }

    public TransformBlock<PacketData, DictionaryPacket> CreateDecoderBlock()
    {
        return new TransformBlock<PacketData, DictionaryPacket>(packet =>
        {
            DictionaryPacket decodedPacket = new DictionaryPacket();

            foreach (IcdField icdField in _icd)
            {
                decodedPacket.Fields[icdField.Name] = DecodeField(packet, icdField);
            }


            return decodedPacket;
        });
    }
    public double DecodeField(PacketData packet, IcdField icdField)
    {
        int absoluteOffset = ConstantPackets.HEADER_BIT_OFFSET + icdField.BitOffset;
        ulong rawValue = _bitEncoder.ReadBits(packet.PayloadPacket, absoluteOffset, icdField.SizeBits);
        return rawValue * icdField.Scale + icdField.Min;
    }

}
