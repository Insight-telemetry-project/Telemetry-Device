using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Interface.Kafka;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services.Kafka;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class PacketPipelineService
    {
        private readonly IBuilderBlock _builderBlock;
        private readonly IDecoderBlock _decoderBlock;
        private readonly KafkaProducerBlock _kafkaProducerBlock;

        public PacketPipelineService(
            IBuilderBlock builderBlock,
            IDecoderBlock decoderBlock,
            KafkaProducerBlock kafkaProducerBlock)
        {
            _builderBlock = builderBlock;
            _decoderBlock = decoderBlock;
            _kafkaProducerBlock = kafkaProducerBlock;
        }

        public async IAsyncEnumerable<DecodedFieldsPacket> RunPipelineStreamAsync(string pcapFilePath)
        {
            LinkBlocks(_builderBlock.Output, _decoderBlock.Input);

            TransformBlock<DecodedFieldsPacket, string> jsonConverter =
                new TransformBlock<DecodedFieldsPacket, string>(
                    decodedPacket => JsonSerializer.Serialize(decodedPacket));

            LinkBlocks(_decoderBlock.Output, jsonConverter);

            LinkBlocks(jsonConverter, _kafkaProducerBlock.KafkaBlock);

            await _builderBlock.Input.SendAsync(pcapFilePath);
            _builderBlock.Input.Complete();

            while (await _decoderBlock.Output.OutputAvailableAsync())
            {
                DecodedFieldsPacket decodedPacket = await _decoderBlock.Output.ReceiveAsync();
                yield return decodedPacket;
            }

            await _decoderBlock.Output.Completion;
            await jsonConverter.Completion;
        }


        private static void LinkBlocks<T>(
    ISourceBlock<T> source,
    ITargetBlock<T> target)
        {
            source.LinkTo(target, new DataflowLinkOptions { PropagateCompletion = true });
        }


    }

}
