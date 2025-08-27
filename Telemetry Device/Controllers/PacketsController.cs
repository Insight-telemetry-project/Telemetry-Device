using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services;

namespace Telemetry_Device.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacketsController : ControllerBase
    {
        private readonly IPcapProcessor _pcapProcessor;
        private readonly DecoderBlock _decoder;
        private readonly IWebHostEnvironment _env;

        public PacketsController(IPcapProcessor pcapProcessor, DecoderBlock decoder, IWebHostEnvironment env)
        {
            _pcapProcessor = pcapProcessor;
            _decoder = decoder;
            _env = env;
        }

        [HttpGet("check-local")]
        public async Task<IActionResult> CheckLocal([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("fileName is required");

            var fullPath = Path.Combine(_env.ContentRootPath, "Common", "Data", "5.pcap");
            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File not found: {fullPath}");

            var packets = _pcapProcessor.ReadPacketsFromFile(fullPath);
            var decoderBlock = _decoder.CreateDecoderBlock();

            int count = 0;
            foreach (var packet in packets)
            {
                if (count++ >= 5) break;
                await decoderBlock.SendAsync(packet);
            }
            decoderBlock.Complete();

            var decodedList = new List<DictionaryPacket>();
            while (await decoderBlock.OutputAvailableAsync())
            {
                while (decoderBlock.TryReceive(out var decoded))
                {
                    decodedList.Add(decoded);
                }
            }

            return Ok(decodedList);
        }
    }
}
