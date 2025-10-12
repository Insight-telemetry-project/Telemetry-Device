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

        public async IAsyncEnumerable<DecodedFieldsPacket> RunPipelineStreamAsync(string pcapFilePath)
        {
            LinkBlocks(_builderBlock.Output, _decoderBlock.Input);

            await _builderBlock.Input.SendAsync(pcapFilePath);
            _builderBlock.Input.Complete();

            while (await _decoderBlock.Output.OutputAvailableAsync())
            {
                DecodedFieldsPacket decodedPacket = await _decoderBlock.Output.ReceiveAsync();
                yield return decodedPacket;
            }

            await _decoderBlock.Output.Completion;
        }
        private void LinkBlocks(ISourceBlock<PacketData> builderOutput,
                                ITargetBlock<PacketData> decoderInput)
        {
            builderOutput.LinkTo(decoderInput, new DataflowLinkOptions
            {
                PropagateCompletion = true
            });
        }
    }
}
