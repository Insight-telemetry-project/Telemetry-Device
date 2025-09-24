using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Models.Interface.TplBlocks
{
    public interface IBuilderBlock
    {
        BufferBlock<PacketData> CreateBuilderBlock(string filePath);

        PacketData ReadSinglePacket(BinaryReader reader);

    }
}
