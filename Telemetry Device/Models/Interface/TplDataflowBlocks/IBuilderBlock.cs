using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Models.Interface.TplBlocks
{
    public interface IBuilderBlock
    {
        ITargetBlock<string> Input { get; }
        ISourceBlock<PacketData> Output { get; }

        PacketData ReadSinglePacket(BinaryReader reader);

    }
}
