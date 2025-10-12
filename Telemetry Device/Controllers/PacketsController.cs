using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Interface.Files;
using Telemetry_Device.Models.Interface.Kafka;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Services.Files;
using Telemetry_Device.Services.TplDataflowBlocks;

namespace Telemetry_Device.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacketsController : ControllerBase
    {
        private readonly PacketPipelineService _pipeline;
        private readonly IKafkaSendMessage _kafka;
        private readonly IFileOperations _fileOperations;

        public PacketsController(PacketPipelineService pipeline, IKafkaSendMessage kafka, IFileOperations fileOperations)
        {
            _pipeline = pipeline;
            _kafka = kafka;
            _fileOperations = fileOperations;
        }
        
        [HttpPost("decode-stream-file")]
        public async IAsyncEnumerable<DecodedFieldsPacket> DecodeStream([FromForm, Required] IFormFile pcapFile)
        {
            string tempFilePath = _fileOperations.GetFullPath(pcapFile.FileName);
            using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await pcapFile.CopyToAsync(stream);
            }
            await foreach (DecodedFieldsPacket packet in _pipeline.RunPipelineStreamAsync(tempFilePath))
            {
                yield return packet;
            }
            System.IO.File.Delete(tempFilePath);
        }
       
    }

}

