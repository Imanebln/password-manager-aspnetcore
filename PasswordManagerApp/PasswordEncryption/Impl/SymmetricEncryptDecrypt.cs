using PasswordEncryption.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace PasswordEncryption.Impl
{
    public class SymmetricEncryptDecrypt : ISymmetricEncryptDecrypt
    {
        public byte[] InitializeSymmetricEncryptionKeyIV()
        {
            return GenerateRandomBytes(10);
        }
        public byte[] InitSymmetricEncryptionKeyIV(string password, byte[] k)
        {
            /* Rfc2898DeriveBytes.Pbkdf2(password, k, iterations: 50000, HashAlgorithmName.SHA256, outputLength: 10);*/
            
            Console.WriteLine(Convert.ToBase64String(k));
            Console.WriteLine(Convert.ToBase64String(HMACSHA256.HashData(k, Convert.FromBase64String(password))));
            return HMACSHA256.HashData(k, Convert.FromBase64String(password));
           
            /*Console.WriteLine(Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(password, k, iterations: 50000, HashAlgorithmName.SHA256, outputLength: 10)));*/
        }
        private string GetEncodedRandomString(int length)
        {
            var base64 = Convert.ToBase64String(GenerateRandomBytes(length));
            return base64;
        }
        private byte[] GenerateRandomBytes(int length)
        {
            var byteArray = new byte[length];
            RandomNumberGenerator.Fill(byteArray);
            // or RandomNumberGenerator.GetBytes(length);
            return byteArray;
        }
        private Aes CreateCipher(string keyBase64)
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Convert.FromBase64String(keyBase64);

            return cipher;
        }
        public string Encrypt(string text, string IV, string key)
        {
            Aes cipher = CreateCipher(key);
           /* cipher.IV = Convert.FromBase64String(IV);*/

            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return Convert.ToBase64String(cipherText);
        }
        public string Decrypt(string encryptedText, string IV, string key)
        {
            Aes cipher = CreateCipher(key);
            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
