namespace PasswordManager.DTO
{
    public class DecryptModel
    {
        public string EncryptedPassword { get; set; } = string.Empty;
        public string IVBase64 { get; set; } = string.Empty ;
        public string Key { get; set; } = string.Empty;
    }
}
