namespace Telemetry_Device.Models
{
    public class IcdField
    {
        public string Name { get; set; }
        public string Units { get; set; }
        public int Correlator { get; set; }
        public string Mask { get; set; }
        public int ByteLocation { get; set; }
        public int SizeBits { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
