﻿using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DataAccess
{
    public class UserDataRepository : IUserDataRepository
    {
        private readonly IMongoDbDataAccess _dataAccess;

        public UserDataRepository(IMongoDbDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<UserDataModel>> GetAllData()
        {
            return await _dataAccess.GetAllRecords<UserDataModel>("Users_data");
        }

        public async Task<UserDataModel> GetDataById(Guid id)
        {
            return await _dataAccess.GetRecordById<UserDataModel>("Users_data", id);
        }

        public async Task InsertData(UserDataModel userData)
        {
            await _dataAccess.InsertRecord("Users_data", userData);
        }

        public async Task UpdateData(UserDataModel userData, Guid id)
        {
            await _dataAccess.UpdateRecord("Users_data", id, userData);
        }

        public async Task DeleteData(Guid id)
        {
            await _dataAccess.DeleteRecord<UserDataModel>("Users_data", id);
        }
    }
}