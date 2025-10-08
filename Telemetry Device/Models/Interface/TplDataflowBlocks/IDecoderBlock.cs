using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Interface.Icd;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Models.Interface.TplDataflowBlocks
{
    public interface IDecoderBlock
    {
        double DecodeFieldValue(PacketData packet, IcdField icdField);
        ITargetBlock<PacketData> Input { get; }
        ISourceBlock<DictionaryPacket> Output { get; }

       

    }
}
