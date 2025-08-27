using System.Collections.Generic;
using Telemetry_Device.Models.Packets;

namespace Telemetry_Device.Models.Interface
{
    public interface IPcapProcessor
    {
        IEnumerable<PacketData> ReadPacketsFromFile(string filePath);
    }
}
