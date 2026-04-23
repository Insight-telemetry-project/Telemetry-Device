using SendRecieveUDP.Model.Constant;
using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Service.BitManipulation;
using Telemetry_Device.Models.Configuration;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Interface.Files;
using Telemetry_Device.Models.Interface.Icd;
using Telemetry_Device.Models.Interface.Kafka;
using Telemetry_Device.Models.Interface.Networking;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Models.Mongo;
using Telemetry_Device.Services.Files;
using Telemetry_Device.Services.Icd;
using Telemetry_Device.Services.Kafka;
using Telemetry_Device.Services.Mongo;
using Telemetry_Device.Services.Networking;
using Telemetry_Device.Services.TplDataflowBlocks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection(MongoSettings.SectionName));

builder.Services.AddSingleton<IBitOperations, BitOperations>();
builder.Services.AddSingleton<IIcdProvider, IcdFileProvider>();

builder.Services.AddScoped<IBuilderBlock, BuilderBlock>();
builder.Services.AddScoped<IDecoderBlock, DecoderBlock>();
builder.Services.AddScoped<PacketPipelineService>();

builder.Services.AddSingleton<IFileOperations, FileOperations>();
builder.Services.AddSingleton<IUdpChecksumCalculator, UdpChecksumCalculator>();


builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<IKafkaSendMessage, KafkaSendMessage>();

builder.Services.AddScoped<KafkaProducerBlock>();



builder.Services.Configure<FlightHeaderSettings>(
    builder.Configuration.GetSection(FlightHeaderSettings.SectionName));

builder.Services.AddSingleton<FlightTelemetryMongoProxy>();
builder.Services.AddScoped<PacketCountingBlock>();
builder.Services.AddHttpClient<IFlightCompletionNotifier, FlightCompletionNotifier>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MongoConsumer:BaseUrl"]!);
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseCors("AngularDev");

app.MapControllers();

app.Run();
