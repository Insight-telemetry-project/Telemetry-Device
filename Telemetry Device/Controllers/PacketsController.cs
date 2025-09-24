using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services.TplDataflowBlocks;

namespace Telemetry_Device.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacketsController : ControllerBase
    {
        private readonly PacketPipelineService _pipeline;

        public PacketsController(PacketPipelineService pipeline)
        {
            _pipeline = pipeline;
        }

        [HttpGet("decode-local")]
        public async Task<IActionResult> DecodeLocal([FromQuery] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return BadRequest("fileName is required");


            string fullPath = Path.Combine( "Data", fileName);


            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File not found: {fullPath}");

            List<DictionaryPacket> decodedList = await _pipeline.RunAsync(fullPath);

            return Ok(decodedList);
        }
    }
}
