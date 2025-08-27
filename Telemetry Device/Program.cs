using SendRecieveUDP.Model.Interfaces.BitManipulation;
using SendRecieveUDP.Service.BitManipulation;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IBitEncoder, BitEncoder>();
builder.Services.AddSingleton<IPcapProcessor, BuilderBlock>();

builder.Services.AddSingleton<DecoderBlock>(sp =>
{
    var bitEncoder = sp.GetRequiredService<IBitEncoder>();
    var icdJson = File.ReadAllText("Common/Data/icd.json");
    var icd = JsonSerializer.Deserialize<List<IcdField>>(icdJson)!;
    return new DecoderBlock(bitEncoder, icd);
});

var app = builder.Build();

var processor = app.Services.GetRequiredService<IPcapProcessor>();
var decoder = app.Services.GetRequiredService<DecoderBlock>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();



var decodeBlock = decoder.CreateDecoderBlock();

var printBlock = new ActionBlock<DictionaryPacket>(decoded =>
{
    Console.WriteLine("Decoded Packet:");
    foreach (var kv in decoded.Fields)
    {
        Console.WriteLine($"  {kv.Key}: {kv.Value}");
    }
    Console.WriteLine();
});

decodeBlock.LinkTo(printBlock, new DataflowLinkOptions { PropagateCompletion = true });

foreach (var packet in processor.ReadPacketsFromFile(@"Common/Data/1.pcap"))
{
    decodeBlock.Post(packet);
}

decodeBlock.Complete();
await printBlock.Completion;


app.Run();
