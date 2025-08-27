namespace Telemetry_Device.Models.Packets
{
    public class DictionaryPacket
    {
        public Dictionary<string, double> Fields { get; } = new Dictionary<string, double>();
    }
}
