using SendRecieveUDP.Model.Interfaces.BitManipulation;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;

public class DecoderBlock : IDecoderBlock
{
    private readonly IBitEncoder _bitEncoder;
    private readonly List<IcdField> _icd;


    public DecoderBlock(IBitEncoder bitEncoder, List<IcdField> icd)
    {
        _bitEncoder = bitEncoder;
        _icd = icd;
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
