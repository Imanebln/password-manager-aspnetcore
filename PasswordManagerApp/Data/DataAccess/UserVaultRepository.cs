using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess
{
    public class UserVaultRepository : IUserVaultRepository
    {
        private readonly IMongoDbDataAccess _db;

        public UserVaultRepository(IMongoDbDataAccess db)
        {
            _db = db;
        }
        public async Task<IEnumerable<AccountInfosModel>> GetUserVault(Guid userId)
        {
            var userData = await _db.GetRecordByPropValue<UserDataModel, Guid>("Users_data", userId, "UserId");
            if (userData is null)
                throw new Exception("User Data not found.");

            return userData.AccountInfos!;
        }

        public async Task<AccountInfosModel?> GetPasswordById(Guid id,Guid userId)
        {
            var userData = await _db.GetRecordByPropValue<UserDataModel, Guid>("Users_data", userId, "UserId");
            if (userData is null)
                throw new Exception("User Data not found.");

            return userData.AccountInfos!.Where(ai => ai.Id.Equals(id)).FirstOrDefault();
        }

        public async Task UpdatePasswordById(Guid id, Guid userId,AccountInfosModel infosModel)
        {
            var userData = await _db.GetRecordByPropValue<UserDataModel, Guid>("Users_data", userId, "UserId");
            if (userData is null)
                throw new Exception("User Data not found.");

            if (userData.AccountInfos is null)
                userData.AccountInfos = new List<AccountInfosModel>();

            var password = userData.AccountInfos!.Where(ai => ai.Id.Equals(id)).FirstOrDefault();
            if (password is null)
                throw new Exception("Password not found.");

            userData.AccountInfos = userData.AccountInfos.Select(ai => ai.Id.Equals(id) ? infosModel : ai );

            await _db.UpdateRecord("Users_data", userData.Id, userData);
        }

        public async Task InsertPassword(AccountInfosModel accountInfos, Guid userId)
        {
            var userData = await _db.GetRecordByPropValue<UserDataModel, Guid>("Users_data", userId, "UserId");
            if (userData is null)
                throw new Exception("User Data not found.");

            if (userData.AccountInfos is null)
                userData.AccountInfos = new List<AccountInfosModel>();

            var passwords = userData.AccountInfos.ToList();
            passwords.Add(accountInfos);

            userData.AccountInfos = passwords;

            await _db.UpdateRecord("Users_data", userData.Id, userData);
        }

        public async Task DeletePassword(Guid id,Guid userId)
        {
            var userData = await _db.GetRecordByPropValue<UserDataModel, Guid>("Users_data", userId, "UserId");
            if (userData is null)
                throw new Exception("User Data not found.");

            if (userData.AccountInfos is null)
                return;

            var passwords = userData.AccountInfos.ToList();
            var itemToRemove = passwords.Where(a => a.Id.Equals(id)).SingleOrDefault();
            if (itemToRemove is null)
                throw new Exception("Password not found.");

            passwords.Remove(itemToRemove);
            userData.AccountInfos = passwords;

            await _db.UpdateRecord("Users_data", userData.Id, userData);
        }
    }
}
