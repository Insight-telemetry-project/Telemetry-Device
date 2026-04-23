using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Telemetry_Device.Models.Configuration;
using Telemetry_Device.Models.Interface.Kafka;

namespace Telemetry_Device.Services.Kafka
{
    public class KafkaSendMessage : IKafkaSendMessage
    {
        private readonly IOptions<KafkaSettings> _settings;
        private IProducer<Null, string>? _producer;

        public KafkaSendMessage(IOptions<KafkaSettings> settings)
        {
            _settings = settings;
        }

        private IProducer<Null, string> Producer =>
            _producer ??= new ProducerBuilder<Null, string>(new ProducerConfig
            {
                BootstrapServers = _settings.Value.BootstrapServers
            }).Build();

        public async Task SendMessageAsync(string topic, string message)
        {
            try
            {
                await Producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            }
            catch (ProduceException<Null, string> exception)
            {
                Console.WriteLine($"Delivery failed: {exception.Error.Reason}");
            }
        }
    }
}
