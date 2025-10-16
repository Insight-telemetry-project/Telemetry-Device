using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry_Device.Models.Schema
{
    public class TelemetryRecord
    {
        [BsonElement("Master Index")]
        public int MasterIndex { get; set; }

        [BsonElement("Fields")]
        public Dictionary<string, int> Fields { get; set; } = new();
    }
}
