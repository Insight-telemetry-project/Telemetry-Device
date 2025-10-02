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

        public async Task StartPipelineAsync(string filePath)
        {
            BufferBlock<PacketData> builder = _builderBlock.CreateBuilderBlock(filePath);
            TransformBlock<PacketData, DictionaryPacket> decoder = _decoderBlock.CreateDecoderBlock();

            builder.LinkTo(decoder, new DataflowLinkOptions { PropagateCompletion = true });

            TransformBlock<DictionaryPacket, string> serializeBlock =
                new TransformBlock<DictionaryPacket, string>(decoded => JsonSerializer.Serialize(decoded));

            decoder.LinkTo(serializeBlock, new DataflowLinkOptions { PropagateCompletion = true });
            serializeBlock.LinkTo(_kafkaProducerBlock.KafkaBlock, new DataflowLinkOptions { PropagateCompletion = true });

            await _kafkaProducerBlock.KafkaBlock.Completion;
        }
    }

}
