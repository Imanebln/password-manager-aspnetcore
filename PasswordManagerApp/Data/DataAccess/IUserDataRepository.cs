using Data.Models;

namespace Data.DataAccess
{
    public interface IUserDataRepository
    {
        Task DeleteData(Guid id);
        Task<IEnumerable<UserDataModel>> GetAllData();
        Task<UserDataModel> GetDataById(Guid id);
        Task InsertData(UserDataModel userData);
        Task UpdateData(UserDataModel userData, Guid id);
    }
}