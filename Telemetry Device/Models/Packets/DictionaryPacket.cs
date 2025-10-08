namespace Telemetry_Device.Models.Packets
{
    public class DictionaryPacket
    {
        /// Key   - (string) = field name
        /// Value - (double) = decoded numeric value
        public Dictionary<string, double> Fields { get; }
        public DictionaryPacket(Dictionary<string, double> fields)
        {
            Fields = fields;
        }
    }
}
