namespace Telemetry_Device.Models.Mongo
{
    public class FlightHeaderSettings
    {
        public const string SectionName = "FlightHeader";

        public List<string> FlightHeader { get; set; } = new();
    }
}
