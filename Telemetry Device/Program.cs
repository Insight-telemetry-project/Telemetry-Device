using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Service.BitManipulation;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Files;
using Telemetry_Device.Models.Interface.Icd;
using Telemetry_Device.Models.Interface.Kafka;
using Telemetry_Device.Models.Interface.Networking;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Services.Kafka;
using Telemetry_Device.Services.Files;
using Telemetry_Device.Services.Icd;
using Telemetry_Device.Services.Networking;
using Telemetry_Device.Services.TplDataflowBlocks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IBitOperations, BitOperations>();
builder.Services.AddSingleton<IIcdProvider, IcdFileProvider>();

builder.Services.AddScoped<IBuilderBlock, BuilderBlock>();
builder.Services.AddScoped<IDecoderBlock, DecoderBlock>();
builder.Services.AddScoped<PacketPipelineService>();

builder.Services.AddSingleton<IFileOperations, FileOperations>();
builder.Services.AddSingleton<IUdpChecksumCalculator, UdpChecksumCalculator>();
builder.Services.AddSingleton<IKafkaSendMessage>(serviceProvider =>
    new KafkaSendMessage(ConstantKafka.KAFKA_ADDRESS));

builder.Services.AddSingleton<KafkaProducerBlock>(serviceProvider =>
{
    var kafkaService = serviceProvider.GetRequiredService<IKafkaSendMessage>();
    return new KafkaProducerBlock(kafkaService, ConstantKafka.TOPIC_NAME);
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
