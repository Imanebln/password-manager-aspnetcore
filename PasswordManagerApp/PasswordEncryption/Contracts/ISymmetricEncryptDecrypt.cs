

namespace PasswordEncryption.Contracts
{
    public interface ISymmetricEncryptDecrypt
    {

        public (string Key, string IVBase64) InitSymmetricEncryptionKeyIV();
        public string Encrypt(string text, string IV, string key);
        public string Decrypt(string encryptedText, string IV, string key);
        string GenerateRandomUserKey();
        string DeriveKeyFromPassword(string password);
        string EncryptUserKey(string userKey, string encryptionKey);
        string DecryptUserKey(string encryptedUserKey, string decryptionKey);
    }
}
