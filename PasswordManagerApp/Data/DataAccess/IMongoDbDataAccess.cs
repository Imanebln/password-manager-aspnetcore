
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.DataAccess
{
    public interface IMongoDbDataAccess
    {
        Task<DeleteResult> DeleteRecord<T>(string table, Guid id);
        Task<List<T>> GetAllRecords<T>(string table);
        Task<T> GetRecordById<T>(string table, Guid id);
        Task<T> GetRecordByPropValue<T, U>(string table, U value, string propName);
        Task<DeleteResult> DeleteRecordByPropValue<T, U>(string table, U value, string propName);
        Task<ReplaceOneResult> UpdateDataByPropValue<T, U>(string table, U value, T record, string propName) where U : BsonValue;
        Task InsertRecord<T>(string table, T record);
        Task<ReplaceOneResult> UpdateRecord<T>(string table, Guid id, T record);
    }
}