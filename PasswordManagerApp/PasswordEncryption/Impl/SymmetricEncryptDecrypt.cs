using PasswordEncryption.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace PasswordEncryption.Impl
{
    public class SymmetricEncryptDecrypt : ISymmetricEncryptDecrypt
    {
        public (string Key, string IVBase64) InitSymmetricEncryptionKeyIV()
        {
            var key = GetEncodedRandomString(16);
            Aes cipher = CreateCipher(key);
            var IVBase64 = Convert.ToBase64String(cipher.IV);
            return (key, IVBase64);
        }

        /// <summary>
        /// Encrypt using AES
        /// </summary>
        /// <param name="text">any text</param>
        /// <param name="IV">Base64 IV string/param>
        /// <param name="key">Base64 key</param>
        /// <returns>Returns an encrypted string</returns>
        public string Encrypt(string text, string IV, string key)
        {
            Aes cipher = CreateCipher(key);
            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return Convert.ToBase64String(cipherText);
        }

        /// <summary>
        /// Decrypt using AES
        /// </summary>
        /// <param name="text">Base64 string for an AES encryption</param>
        /// <param name="IV">Base64 IV string/param>
        /// <param name="key">Base64 key</param>
        /// <returns>Returns a string</returns>
        public string Decrypt(string encryptedText, string IV, string key)
        {
            Aes cipher = CreateCipher(key);
            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        public string GenerateIVFromKey(string key)
        {
            Aes cipher = CreateCipher(key);
            return Convert.ToBase64String(cipher.IV);
        }

        private string GetEncodedRandomString(int length)
        {
            var base64 = Convert.ToBase64String(GenerateRandomBytes(length));
            return base64;
        }

        /// <summary>
        /// Create an AES Cipher using a base64 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>AES</returns>
        private Aes CreateCipher(string keyBase64)
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Convert.FromBase64String(keyBase64);

            return cipher;
        }

        private byte[] GenerateRandomBytes(int length)
        {
            var byteArray = new byte[length];
            RandomNumberGenerator.Fill(byteArray);

            return byteArray;
        }

        /// <summary>
        /// Generate a random key for a user.
        /// </summary>
        /// <returns>string of random generated key.</returns>
        public string GenerateRandomUserKey()
        {
            return GetEncodedRandomString(16);
        }

        /// <summary>
        /// Generate a kaye derived from a password
        /// </summary>
        /// <param name="password">Password of user.</param>
        /// <returns>A string of key derived from password.</returns>
        public (string derivedKey, string IVBase64) DeriveKeyFromPassword(string password)
        {
            UnicodeEncoding UE = new();

            byte[] passwordBytes = UE.GetBytes(password);
            byte[] aesKey = SHA256.Create().ComputeHash(passwordBytes);
            string derivedKey = Convert.ToBase64String(aesKey);

            Aes cipher = CreateCipher(Convert.ToBase64String(aesKey));
            var IVBase64 = Convert.ToBase64String(cipher.IV);
            return (derivedKey, IVBase64);
        }

        /// <summary>
        /// Encrypt user key using the password derived key.
        /// </summary>
        /// <param name="userKey">User key to encrypt</param>
        /// <param name="encryptionKey">Encryption key used to encrypt the user key.</param>
        /// <returns>string of encrypted user key.</returns>
        public string EncryptUserKey(string userKey, string derivedKey, string encryptionIVBase64)
        {
            //TODO: Implement method
            return Encrypt(userKey, encryptionIVBase64, derivedKey);
        }

        /// <summary>
        /// Decrypt user key using the password derived key.
        /// </summary>
        /// <param name="encryptedUserKey">Encrypted user key to decrypt</param>
        /// <param name="decryptionKey">Encryption key used to decrypt the user key.</param>
        /// <returns>string of decrypted user key.</returns>
        public string DecryptUserKey(string encryptedUserKey, string derivedKey, string encryptionIVBase64)
        {
            //TODO: Implement method.
            return Decrypt(encryptedUserKey, encryptionIVBase64, derivedKey);
        }
    }
}
