using Confluent.Kafka;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Kafka;
using Telemetry_Device.Services.Kafka;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class KafkaProducerBlock
    {
        private readonly ActionBlock<string> _block;
        public KafkaProducerBlock(IKafkaSendMessage kafka)
        {
            _block = new ActionBlock<string>(async message =>
            {
                await kafka.SendMessageAsync(ConstantKafka.TOPIC_NAME, message);
            });
        }

        public ITargetBlock<string> KafkaBlock => _block;
    }
}
