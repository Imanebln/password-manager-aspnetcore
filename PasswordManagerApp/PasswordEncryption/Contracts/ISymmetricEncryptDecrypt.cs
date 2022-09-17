

namespace PasswordEncryption.Contracts
{
    public interface ISymmetricEncryptDecrypt
    {

        public (string Key, string IVBase64) InitSymmetricEncryptionKeyIV();
        public string Encrypt(string text, string IV, string key);
        public string Decrypt(string encryptedText, string IV, string key);
        string GenerateRandomUserKey();
        (string derivedKey, string IVBase64) DeriveKeyFromPassword(string password);
        string EncryptUserKey(string userKey, string derivedKey, string encryptionIVBase64);
        string DecryptUserKey(string encryptedUserKey, string derivedKey, string encryptionIVBase64);
        string GenerateIVFromKey(string key);
    }
}
