using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Interface.Networking;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services.Kafka;
using Telemetry_Device.Services.Mongo;

namespace Telemetry_Device.Services.TplDataflowBlocks
{
    public class PacketPipelineService
    {
        private readonly IBuilderBlock _builderBlock;
        private readonly IDecoderBlock _decoderBlock;
        private readonly KafkaProducerBlock _kafkaProducerBlock;
        private readonly FlightTelemetryMongoProxy _telemetryMongo;
        private readonly PacketCountingBlock _countingBlock;
        private readonly IFlightCompletionNotifier _notifier;
        public PacketPipelineService(
            IBuilderBlock builderBlock,IDecoderBlock decoderBlock,KafkaProducerBlock kafkaProducerBlock,
            FlightTelemetryMongoProxy flightTelemetryMongo,PacketCountingBlock countingBlock, IFlightCompletionNotifier flightCompletionNotifier)
        {
            _builderBlock = builderBlock;
            _decoderBlock = decoderBlock;
            _kafkaProducerBlock = kafkaProducerBlock;
            _telemetryMongo = flightTelemetryMongo;
            _countingBlock = countingBlock;
            _notifier = flightCompletionNotifier;
        }

        public async Task<int> RunPipelineStreamAsync(string pcapFilePath)
        {
            await _telemetryMongo.InitializeCacheAsync();
            LinkBlocks(_builderBlock.Output, _countingBlock.Input);
            LinkBlocks(_countingBlock.Output, _decoderBlock.Input);

            TransformBlock<DecodedFieldsPacket, string> jsonConverter =
                new TransformBlock<DecodedFieldsPacket, string>(
                    decodedPacket => JsonSerializer.Serialize(decodedPacket),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = Environment.ProcessorCount,
                        EnsureOrdered = false
                    });

            LinkBlocks(_decoderBlock.Output, jsonConverter);
            LinkBlocks(jsonConverter, _kafkaProducerBlock.KafkaBlock);

            await _builderBlock.Input.SendAsync(pcapFilePath);
            _builderBlock.Input.Complete();

            await _kafkaProducerBlock.KafkaBlock.Completion;
            await jsonConverter.Completion;

            int masterIndex = _decoderBlock.MasterIndex.Value;
            int totalPackets = _countingBlock.PacketCount;

            await _notifier.NotifyFlightCompletedAsync(masterIndex, totalPackets);

            return masterIndex;
        }

        private static void LinkBlocks<T>(ISourceBlock<T> source, ITargetBlock<T> target)
        {
            source.LinkTo(target, new DataflowLinkOptions { PropagateCompletion = true });
        }
    }
}
