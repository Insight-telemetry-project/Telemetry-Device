using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;
using Telemetry_Device.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register DI
builder.Services.AddSingleton<FieldMapping>();
builder.Services.AddSingleton<Decoder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Resolve decoder from DI
var decoderService = app.Services.GetRequiredService<Decoder>();

// Pipeline blocks
var readerBlock = PcapProcessor.CreatePcapReaderBlock();
var explodeBlock = new TransformManyBlock<List<Packet>, Packet>(list => list);
var decoderBlock = decoderService.CreateDecoderBlock();

var printerBlock = new ActionBlock<DecodedPacket>(decoded =>
{
    Console.WriteLine($"Timestamp: {decoded.Timestamp}");
    Console.WriteLine($"Volt1: {decoded.Volt1}, Volt2: {decoded.Volt2}");
    Console.WriteLine($"Amp1: {decoded.Amp1}, Amp2: {decoded.Amp2}");
    Console.WriteLine($"FQtyL: {decoded.FQtyL}, FQtyR: {decoded.FQtyR}");
    Console.WriteLine($"E1FFlow: {decoded.E1FFlow}, E1OilT: {decoded.E1OilT}, E1OilP: {decoded.E1OilP}, E1RPM: {decoded.E1RPM}");
    Console.WriteLine($"CHT1: {decoded.E1CHT1}, CHT2: {decoded.E1CHT2}, CHT3: {decoded.E1CHT3}, CHT4: {decoded.E1CHT4}");
    Console.WriteLine($"EGT1: {decoded.E1EGT1}, EGT2: {decoded.E1EGT2}, EGT3: {decoded.E1EGT3}, EGT4: {decoded.E1EGT4}");
    Console.WriteLine($"OAT: {decoded.OAT}, IAS: {decoded.IAS}, VSpd: {decoded.VSpd}, NormAc: {decoded.NormAc}, AltMSL: {decoded.AltMSL}");
    Console.WriteLine($"Timestep: {decoded.Timestep}, Cluster: {decoded.Cluster}, MasterIndex: {decoded.MasterIndex}, DateDiff: {decoded.DateDiff}, FlightLength: {decoded.FlightLength}, NumberFlightsBefore: {decoded.NumberFlightsBefore}");
    Console.WriteLine("------------------------------------------------------");
});

// Link pipeline
readerBlock.LinkTo(explodeBlock, new DataflowLinkOptions { PropagateCompletion = true });
explodeBlock.LinkTo(decoderBlock, new DataflowLinkOptions { PropagateCompletion = true });
decoderBlock.LinkTo(printerBlock, new DataflowLinkOptions { PropagateCompletion = true });

// Kickoff
string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "index_23517_time_17673.pcap");
readerBlock.Post(filePath);
readerBlock.Complete();

printerBlock.Completion.ContinueWith(_ =>
{
    Console.WriteLine("Processing completed.");
});

app.Run();
