using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Service.BitManipulation;
using System.Text.Json;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Interface.Networking;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Services.Networking;
using Telemetry_Device.Services.TplDataflowBlocks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddSingleton<IBitEncoder, BitEncoder>();
builder.Services.AddSingleton<IDecoderBlock>(serviceProvider =>
{
    IBitEncoder bitEncoder = serviceProvider.GetRequiredService<IBitEncoder>();

    string icdJson = File.ReadAllText("Data/icd.json");
    List<IcdField> icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson)!;

    return new DecoderBlock(bitEncoder, icd);
});

builder.Services.AddSingleton<IBuilderBlock, BuilderBlock>();
builder.Services.AddSingleton<PacketPipelineService>();
builder.Services.AddSingleton<IUdpChecksumCalculator, UdpChecksumCalculator>();


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
