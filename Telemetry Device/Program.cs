using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Service.BitManipulation;
using System.Text.Json;
using Telemetry_Device.Models.Interface.Icd;
using Telemetry_Device.Models.Interface.TplBlocks;
using Telemetry_Device.Models.Interface.TplDataflowBlocks;
using Telemetry_Device.Services.Icd;
using Telemetry_Device.Services.TplDataflowBlocks;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddSingleton<IBitEncoder, BitEncoder>();
builder.Services.AddSingleton<IIcdProvider, IcdFileProvider>();
builder.Services.AddSingleton<IDecoderBlock, DecoderBlock>();
builder.Services.AddSingleton<IBuilderBlock, BuilderBlock>();
builder.Services.AddSingleton<PacketPipelineService>();


WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
