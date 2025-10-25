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
    public class FlightTelemetryMongoProxy
    {
        private readonly IMongoCollection<FlightTelemetryRecord> telemetryCollection;
        private bool _isIndexCreated = false;

        public FlightTelemetryMongoProxy(IOptions<MongoSettings> mongoOptions)
        {
            MongoSettings mongoSettings = mongoOptions.Value;

            MongoClient mongoClient = new MongoClient(mongoSettings.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(mongoSettings.DatabaseName);

            telemetryCollection = mongoDatabase.GetCollection<FlightTelemetryRecord>(mongoSettings.CollectionName);
        }

        public async Task StoreFlightDataAsync(Dictionary<string, int> fields)
        {
            await EnsureIndexesCreatedAsync();

            int masterIndex = fields[ConstantPackets.FLIGHT_ID];
            fields.Remove(ConstantPackets.FLIGHT_ID);

            bool exists = await telemetryCollection
                .Find(record => record.MasterIndex == masterIndex)
                .AnyAsync();

            if (!exists)
            {
                FlightTelemetryRecord telemetryRecord = new FlightTelemetryRecord
                {
                    MasterIndex = masterIndex,
                    Fields = fields
                };

                await telemetryCollection.InsertOneAsync(telemetryRecord);
            }
        }

        private async Task EnsureIndexesCreatedAsync()
        {
            if (!_isIndexCreated)
            {
                CreateIndexModel<FlightTelemetryRecord> masterIndexModel = new CreateIndexModel<FlightTelemetryRecord>(
                    Builders<FlightTelemetryRecord>.IndexKeys.Ascending(record => record.MasterIndex),
                    new CreateIndexOptions { Unique = true });

                await telemetryCollection.Indexes.CreateOneAsync(masterIndexModel);
                _isIndexCreated = true;
            }
        }


    }
}
