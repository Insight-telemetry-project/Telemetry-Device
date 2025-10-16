using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telemetry_Device.Models.Configuration;
using Telemetry_Device.Models.Constant;
using Telemetry_Device.Models.Schema;

namespace Telemetry_Device.Services.Mongo
{
    public class TelemetryMongo
    {
        private readonly IMongoCollection<TelemetryRecord> telemetryCollection;

        public TelemetryMongo(IOptions<MongoSettings> mongoOptions)
        {
            MongoSettings mongoSettings = mongoOptions.Value;

            MongoClient mongoClient = new MongoClient(mongoSettings.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            telemetryCollection = mongoDatabase.GetCollection<TelemetryRecord>(mongoSettings.CollectionName);

            CreateIndexModel<TelemetryRecord> masterIndexModel =
                new CreateIndexModel<TelemetryRecord>(
                    Builders<TelemetryRecord>.IndexKeys.Ascending(record => record.MasterIndex),
                    new CreateIndexOptions { Unique = true });

            telemetryCollection.Indexes.CreateOne(masterIndexModel);
        }

        public async Task SaveTelemetryAsync(Dictionary<string, int> fields)
        {
            int masterIndex = fields[ConstantPackets.FLIGHT_ID];

            fields.Remove(ConstantPackets.FLIGHT_ID);

            TelemetryRecord telemetryRecord = new TelemetryRecord
            {
                MasterIndex = masterIndex,
                Fields = fields
            };

            await telemetryCollection.UpdateOneAsync(
                Builders<TelemetryRecord>.Filter.Eq(record => record.MasterIndex, masterIndex),
                Builders<TelemetryRecord>.Update
                    .SetOnInsert(record => record.Fields, telemetryRecord.Fields)
                    .SetOnInsert(record => record.MasterIndex, telemetryRecord.MasterIndex),
                new UpdateOptions { IsUpsert = true });
        }
    }
}
