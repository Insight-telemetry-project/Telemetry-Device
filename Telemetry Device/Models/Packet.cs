namespace Telemetry_Device.Models
{
    public class Packet
    {
        public uint TimestampSeconds { get; set; }
        public uint TimestampMicroseconds { get; set; }
        public uint IncludedLength { get; set; }
        public uint OriginalLength { get; set; }
        public byte[] Payload { get; set; }
    }
}
