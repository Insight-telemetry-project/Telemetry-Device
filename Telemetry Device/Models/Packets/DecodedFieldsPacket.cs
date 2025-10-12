namespace Telemetry_Device.Models.Packets
{
    public class DecodedFieldsPacket
    {
        /// Key   - (string) = field name
        /// Value - (double) = decoded numeric value
        public Dictionary<string, double> Fields { get; }
        public DecodedFieldsPacket(Dictionary<string, double> fields)
        {
            Fields = fields;
        }
    }
}
