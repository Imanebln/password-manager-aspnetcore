namespace PasswordManager.DTO.UserData
{
    public class AccountsInfosModelInsertDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
        public string EncryptedPasswordIV { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
