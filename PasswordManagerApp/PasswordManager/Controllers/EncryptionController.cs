using Microsoft.AspNetCore.Mvc;
using PasswordEncryption.Contracts;
using PasswordManager.DTO;

namespace PasswordManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EncryptionController : ControllerBase
    {
        public ISymmetricEncryptDecrypt _symmetricEncryptDecrypt;
        public EncryptionController(ISymmetricEncryptDecrypt symmetricEncryptDecrypt)
        {
            _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
        }

        [HttpGet("GetSymmetricKey")]
        public (string Key, string IVBase64) GetSymmetricKey()
        {
            var (Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();
            Console.WriteLine("Your symmetric key and IV: " + (Key, IVBase64));
            return (Key, IVBase64);
        }

        [HttpGet("EncryptPassword")]
        public string EncryptPassword(string password, string Key, string IVBase64)
        {
            /*var (Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();*/
            Console.WriteLine("Your password before encryption: " + password);
            Console.WriteLine("Your symmetric key and IV: " + (Key, IVBase64));
            var encryptedPassord = _symmetricEncryptDecrypt.Encrypt(password, IVBase64, Key);
            Console.WriteLine("Your encrypted password is: " + encryptedPassord);
            return encryptedPassord;
        }

        [HttpPost("DecryptPassword")]
        public string DecryptPassword(DecryptModel decryptModel)
        {

            var decryptedPassword = _symmetricEncryptDecrypt.Decrypt(decryptModel.EncryptedPassword, decryptModel.IVBase64, decryptModel.Key);
            Console.WriteLine("Your password after decryption: " + decryptedPassword);
            return decryptedPassword;
        }

        // last encryption updates

        [HttpPost("encrypt-decrypt-password-key")]
        public string EncryptPasswordKey()
        {
            string user_key = _symmetricEncryptDecrypt.GenerateRandomUserKey();
            string password = "Admin12@";
            var (DerivedKey, IVBase64) = _symmetricEncryptDecrypt.DeriveKeyFromPassword(password);
            Console.WriteLine("Your password is: " + password);
            Console.WriteLine("Randomly generated user_key is: " + user_key);
            Console.WriteLine("Derived key from your password is: " + DerivedKey);
            Console.WriteLine("IVBase64 key is: " + IVBase64);
            string encrypted_user_key = _symmetricEncryptDecrypt.EncryptUserKey(user_key, DerivedKey, IVBase64);
            Console.WriteLine("Encrypted user_key is: " + encrypted_user_key);
            Console.WriteLine("Decrypted encrypted_user_key is: " + _symmetricEncryptDecrypt.DecryptUserKey(encrypted_user_key, DerivedKey, IVBase64));
            return _symmetricEncryptDecrypt.EncryptUserKey(user_key, DerivedKey, IVBase64);
        }


    }
}
