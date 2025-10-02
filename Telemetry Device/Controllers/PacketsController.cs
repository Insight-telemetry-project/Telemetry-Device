using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Interface.Kafka;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services.TplDataflowBlocks;

namespace Telemetry_Device.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacketsController : ControllerBase
    {
        private readonly PacketPipelineService _pipeline;
        private readonly IKafkaSendMessage _kafka;

        public PacketsController(PacketPipelineService pipeline, IKafkaSendMessage kafka)
        {
            _pipeline = pipeline;
            _kafka = kafka;
        }


        [HttpGet("decode-local")]
        public async Task<IActionResult> DecodeLocal([FromQuery] string fileName)
        {
            string fullPath = Path.Combine("Data", fileName);

            if (!System.IO.File.Exists(fullPath))
                return NotFound($"File not found: {fullPath}");

            await _pipeline.StartPipelineAsync(fullPath);

            return Ok("Packets sent to Kafka");
        }




        [HttpGet("test-kafka")]
        public async Task<IActionResult> TestKafka()
        {
            await _kafka.SendMessageAsync("test-topic", "Hello from Telemetry_Device!");
            return Ok("Kafka test message sent");
        }




    }

}
