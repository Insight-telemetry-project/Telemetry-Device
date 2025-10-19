namespace Telemetry_Device.Models.Configuration
{
    public class MongoSettings
    {
        public const string SectionName = "DbSettings";

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
