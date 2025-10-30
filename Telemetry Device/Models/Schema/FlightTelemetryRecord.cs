using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Bson.Serialization.Attributes;
using static MongoDB.Driver.WriteConcern;

namespace Telemetry_Device.Models.Schema
{
    public class FlightTelemetryRecord
    {
        [BsonElement("Master Index")]
        public int MasterIndex { get; set; }

        [BsonElement("Fields")]
        public Dictionary<string, int> Fields { get; set; } = new();

        [BsonElement("Connections")]
        public Dictionary<string, List<string>> Connections { get; set; } = new();

    }
}
