using Data.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess
{
    public class MongoDbDataAccess : IMongoDbDataAccess
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        public MongoDbDataAccess(MongoDbConfig dbConfig)
        {
            _client = new MongoClient(dbConfig.ConnectionString);
            _database = _client.GetDatabase(dbConfig.DatabaseName);
        }


        public async Task InsertRecord<T>(string table, T record)
        {
            var collection = _database.GetCollection<T>(table);
            await collection.InsertOneAsync(record);

        }

        public async Task<List<T>> GetAllRecords<T>(string table)
        {
            var collection = _database.GetCollection<T>(table);
            return (await collection.FindAsync(new BsonDocument())).ToList();
        }

        public async Task<T> GetRecordById<T>(string table, Guid id)
        {
            var collection = _database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", id);

            return (await collection.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<ReplaceOneResult> UpdateRecord<T>(string table, Guid id, T record)
        {
            var collection = _database.GetCollection<T>(table);
             return await collection.ReplaceOneAsync(new BsonDocument("_id", id), record);
        }

        public async Task<DeleteResult> DeleteRecord<T>(string table, Guid id)
        {
            var collection = _database.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await collection.DeleteOneAsync(filter);
        }
    }
}
