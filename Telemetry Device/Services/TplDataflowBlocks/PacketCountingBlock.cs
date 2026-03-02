using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class PacketCountingBlock
    {
        private readonly TransformBlock<PacketData, PacketData> _block;
        private int _packetCount;

        public PacketCountingBlock()
        {
            _packetCount = 0;

            _block = new TransformBlock<PacketData, PacketData>(
                packet =>
                {
                    _packetCount++;
                    return packet;
                    
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 1,
                    EnsureOrdered = true
                });
        }

        public ITargetBlock<PacketData> Input => _block;
        public ISourceBlock<PacketData> Output => _block;

        public int PacketCount => _packetCount;

        public Task Completion => _block.Completion;
    }
}
