namespace Telemetry_Device.Models.Packets
{
    public class PacketData
    {
        public byte[] Payload { get; set; }
        public ushort Checksum { get; set; }
    }
}

