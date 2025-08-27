namespace Telemetry_Device.Models.Packets
{
    public class PacketData
    {

        public byte[] PayloadPacket { get; set; }

        public ushort ChecksumPacket { get; set; }

        //public ushort ChecksumComputed { get; set; }
    }
}

