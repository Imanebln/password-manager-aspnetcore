using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            return (Key,IVBase64);
        }

        [HttpGet("EncryptPassword")]
        public string EncryptPassword(string password,string Key,string IVBase64)
        {
            /*var (Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();*/
            Console.WriteLine("Your password before encryption: " + password);
            Console.WriteLine("Your symmetric key and IV: " + (Key,IVBase64));
            var encryptedPassord = _symmetricEncryptDecrypt.Encrypt(password, IVBase64, Key);
            Console.WriteLine("Your encrypted password is: " + encryptedPassord);
            return encryptedPassord;
        }

        [HttpPost("DecryptPassword")]
        public string DecryptPassword(DecryptModel decryptModel)
        {
            var decryptedPassword = _symmetricEncryptDecrypt.Decrypt(decryptModel.EncryptedPassword, decryptModel.IVBase64, decryptModel.Key);
            Console.WriteLine("Your password after decryption: " + decryptedPassword);
            return decryptedPassword;        }

    }
}
