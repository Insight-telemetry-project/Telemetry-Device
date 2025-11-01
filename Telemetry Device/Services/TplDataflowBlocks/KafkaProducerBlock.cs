using System;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Kafka;

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
                System.Diagnostics.Debug.WriteLine("[KAFKA] Produced");
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                EnsureOrdered = false
            });
        }

        public ITargetBlock<string> KafkaBlock => _block;
    }
}
