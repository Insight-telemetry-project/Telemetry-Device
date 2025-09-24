using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class PacketPipelineService
    {
        private readonly IBuilderBlock _builderBlock;
        private readonly IDecoderBlock _decoderBlock;

        public PacketPipelineService(IBuilderBlock builderBlock, IDecoderBlock decoderBlock)
        {
            _builderBlock = builderBlock;
            _decoderBlock = decoderBlock;
        }

        public async Task<List<DictionaryPacket>> RunAsync(string filePath)
        {
            BufferBlock<PacketData> builder = _builderBlock.CreateBuilderBlock(filePath);
            TransformBlock<PacketData, DictionaryPacket> decoder = _decoderBlock.CreateDecoderBlock();

            List<DictionaryPacket> results = new List<DictionaryPacket>();
            ActionBlock<DictionaryPacket> collectBlock = new ActionBlock<DictionaryPacket>(decoded => results.Add(decoded));

            builder.LinkTo(decoder, new DataflowLinkOptions { PropagateCompletion = true });
            decoder.LinkTo(collectBlock, new DataflowLinkOptions { PropagateCompletion = true });

            await collectBlock.Completion;

            return results;
        }
    }
}
