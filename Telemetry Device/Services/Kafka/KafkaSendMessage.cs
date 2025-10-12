using Confluent.Kafka;
using System.Diagnostics;
using Telemetry_Device.Models.Interface.Kafka;

namespace Telemetry_Device.Services.Kafka
{
    public class KafkaSendMessage: IKafkaSendMessage
    {
        private readonly IProducer<Null, string> _producer;

        public KafkaSendMessage(string bootstrapServers)
        {
            ProducerConfig config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessageAsync(string topic, string message)
        {
            try
            {
                DeliveryResult<Null, string> delivery = await _producer.ProduceAsync(
                    topic,
                    new Message<Null, string> { Value = message });

                Debug.WriteLine($"Delivered '{delivery.Value}' to {delivery.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> exception)
            {
                Debug.WriteLine($"Delivery failed: {exception.Error.Reason}");

            }
        }
    }
}
