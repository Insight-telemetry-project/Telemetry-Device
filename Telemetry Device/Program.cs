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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
var readerBlock = PcapProcessor.CreatePcapReaderBlock();

app.Run();
