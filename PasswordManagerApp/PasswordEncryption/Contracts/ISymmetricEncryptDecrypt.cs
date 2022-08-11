

namespace PasswordEncryption.Contracts
{
    public interface ISymmetricEncryptDecrypt
    {

        public byte[] InitializeSymmetricEncryptionKeyIV();
        public string Encrypt(string text, string IV, string key);
        public string Decrypt(string encryptedText, string IV, string key);
        public byte[] InitSymmetricEncryptionKeyIV(string password, byte[] k);
    }
}
