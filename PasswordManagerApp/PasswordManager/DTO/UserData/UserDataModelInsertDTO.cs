using Data.Models;

namespace PasswordManager.DTO.UserData
{
    public class UserDataModelInsertDTO
    {
        public IEnumerable<AccountsInfosModelInsertDTO>? AccountInfos { get; set; }
    }
}
