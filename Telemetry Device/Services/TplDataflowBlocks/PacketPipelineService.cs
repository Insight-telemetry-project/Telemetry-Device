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
        private readonly IKafkaSendMessage _kafkaSendMessage;

        public PacketPipelineService(IBuilderBlock builderBlock, IDecoderBlock decoderBlock, IKafkaSendMessage kafkaSendMessage)
        {
            _builderBlock = builderBlock;
            _decoderBlock = decoderBlock;
            _kafkaSendMessage = kafkaSendMessage;
        }

        public async Task RunAsync(string filePath)
        {
            BufferBlock<PacketData> builder = _builderBlock.CreateBuilderBlock(filePath);
            TransformBlock<PacketData, DictionaryPacket> decoder = _decoderBlock.CreateDecoderBlock();

            // builder -> decoder
            builder.LinkTo(decoder, new DataflowLinkOptions { PropagateCompletion = true });

            // decoder -> kafka
            ActionBlock<DictionaryPacket> kafkaBlock = new ActionBlock<DictionaryPacket>(async decoded =>
            {
                string json = JsonSerializer.Serialize(decoded);
                await _kafkaSendMessage.SendMessageAsync("test-topic", json);
            });

            decoder.LinkTo(kafkaBlock, new DataflowLinkOptions { PropagateCompletion = true });

            await kafkaBlock.Completion;
        }





    }
}
