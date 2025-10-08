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
            List<DictionaryPacket> results = new List<DictionaryPacket>();

            ActionBlock<DictionaryPacket> collectBlock = new ActionBlock<DictionaryPacket>(decoded =>
            {
                results.Add(decoded);
            });

            _builderBlock.Output.LinkTo(_decoderBlock.Input, new DataflowLinkOptions { PropagateCompletion = true });
            _decoderBlock.Output.LinkTo(collectBlock, new DataflowLinkOptions { PropagateCompletion = true });

            await _builderBlock.Input.SendAsync(filePath);
            _builderBlock.Input.Complete();

            await collectBlock.Completion;

            return results;
        }
    }
}
