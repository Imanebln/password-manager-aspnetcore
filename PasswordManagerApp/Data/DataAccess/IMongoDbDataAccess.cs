
using MongoDB.Driver;

namespace Data.DataAccess
{
    public interface IMongoDbDataAccess
    {
        Task<DeleteResult> DeleteRecord<T>(string table, Guid id);
        Task<List<T>> GetAllRecords<T>(string table);
        Task<T> GetRecordById<T>(string table, Guid id);
        Task InsertRecord<T>(string table, T record);
        Task<ReplaceOneResult> UpdateRecord<T>(string table, Guid id, T record);
    }
}