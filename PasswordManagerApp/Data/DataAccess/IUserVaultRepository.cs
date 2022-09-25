using Data.Models;

namespace Data.DataAccess
{
    public interface IUserVaultRepository
    {
        Task<AccountInfosModel?> GetPasswordById(Guid id, Guid userId);
        Task<IEnumerable<AccountInfosModel>> GetUserVault(Guid userId);
        Task UpdatePasswordById(Guid id, Guid userId, AccountInfosModel infosModel);
        Task InsertPassword(AccountInfosModel accountInfos, Guid userId);
        Task DeletePassword(Guid id, Guid userId);
    }
}