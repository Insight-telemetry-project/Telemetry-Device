using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Telemetry_Device.Models.Schema
{
    public class FlightTelemetryRecord
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Master Index")]
        public int MasterIndex { get; set; }

        [BsonElement("Fields")]
        public Dictionary<string, int> Fields { get; set; } = new();

        [BsonElement("Connections")]
        public Dictionary<string, List<string>> Connections { get; set; } = new();

    }
}
