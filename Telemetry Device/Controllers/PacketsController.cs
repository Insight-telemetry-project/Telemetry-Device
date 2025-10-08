using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Telemetry_Device.Models;
using Telemetry_Device.Models.Interface;
using Telemetry_Device.Models.Packets;
using Telemetry_Device.Models.Interface.Files;
using Telemetry_Device.Services.TplDataflowBlocks;

namespace Telemetry_Device.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PacketsController : ControllerBase
    {
        private readonly PacketPipelineService _pipeline;
        private readonly IFileOperations _fileOperations;
        public PacketsController(PacketPipelineService pipeline, IFileOperations fileOperations)
        {
            _pipeline = pipeline;
            _fileOperations = fileOperations;
        }

        [HttpGet("decode-stream")]
        public async IAsyncEnumerable<DictionaryPacket> DecodeStream([FromQuery, Required] string pcapFileName)
        {
            string pcapFilePath = _fileOperations.GetFullPath(pcapFileName);
            if (!_fileOperations.FileExists(pcapFileName))
                yield break;
            await foreach (DictionaryPacket packet in _pipeline.RunPipelineStreamAsync(pcapFilePath))
            {
                yield return packet; 
            }
        }

    }
}
