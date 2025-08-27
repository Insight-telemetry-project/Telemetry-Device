namespace Telemetry_Device.Models.Interface
{
    public class IcdField
    {
        public string Name { get; set; }
        public string Units { get; set; }
        public int BitOffset { get; set; }
        public int SizeBits { get; set; }
        public double Scale { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }
}
