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

        /*[HttpGet("EncryptPassword")]
        public string EncryptPassword(string password)
        {
            var (Key, IVBase64) = _symmetricEncryptDecrypt.InitializeSymmetricEncryptionKeyIV();
            Console.WriteLine("Your password before encryption: "+password);
            Console.WriteLine("Your symmetric key and IV: "+(Key, IVBase64));
            Console.WriteLine("Your encrypted password is: " + _symmetricEncryptDecrypt.Encrypt(password, IVBase64, Key));
            return _symmetricEncryptDecrypt.Encrypt(password, IVBase64, Key);

        }*/
        [HttpGet("Encrypt")]
        public void Encrypt(string password)
        {
            var key = _symmetricEncryptDecrypt.InitializeSymmetricEncryptionKeyIV();
            _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV(password,key);
        }

        [HttpPost("DecryptPassword")]
        public string DecryptPassword(DecryptModel decryptModel)
        {
            Console.WriteLine("Your password after decryption: " + _symmetricEncryptDecrypt.Decrypt(decryptModel.EncryptedPassword, decryptModel.IVBase64, decryptModel.Key));
            return _symmetricEncryptDecrypt.Decrypt(decryptModel.EncryptedPassword, decryptModel.IVBase64, decryptModel.Key);
    
        }

    }
}
