using Confluent.Kafka;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Services.Kafka;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class KafkaProducerBlock
    {
        private readonly ActionBlock<string> _block;

        public KafkaProducerBlock(KafkaSendMessage kafka, string topic)
        {
            _block = new ActionBlock<string>(async message =>
            {
                await kafka.SendMessageAsync(topic, message);
            });
        }

        public ITargetBlock<string> Block => _block;
    }
}
