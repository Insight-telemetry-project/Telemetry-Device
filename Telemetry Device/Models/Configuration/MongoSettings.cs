namespace Telemetry_Device.Models.Configuration
{
    public class MongoSettings
    {
        public const string SectionName = "MongoSettings";

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
