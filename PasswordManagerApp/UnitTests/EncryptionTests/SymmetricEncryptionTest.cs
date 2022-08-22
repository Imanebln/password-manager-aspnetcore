using PasswordEncryption.Impl;
using Xunit;
using Xunit.Priority;

namespace UnitTests.EncryptionTests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class SymmetricEncryptionTest
    {
        private readonly SymmetricEncryptDecrypt _encryptionService;
        private readonly string password;
        private readonly string Key;
        private readonly string IVBase64;

        public SymmetricEncryptionTest()
        {
            _encryptionService = new SymmetricEncryptDecrypt();
            (Key,IVBase64) = _encryptionService.InitSymmetricEncryptionKeyIV();
            password = "imane123@";
        }

        [Fact, Priority(1)]
        public void InitSymmetricEncryptionKeyIVTest()
        {
            // Act
            var result = _encryptionService.InitSymmetricEncryptionKeyIV();

            // Assert
            Assert.IsType<(string Key, string IVBase64)>(result);
            
        }
        
        [Fact, Priority(2)]
        public string EncryptTest()
        {
            // Act 
            var result = _encryptionService.Encrypt(password, IVBase64, Key);

            // Assert
            Assert.IsType<string>(result);
            return result;
        }

        [Fact, Priority(3)]
        public void DecryptTest()
        {
            // Act
            var result = _encryptionService.Decrypt(EncryptTest(), IVBase64, Key);

            // Assert
            Assert.IsType<string>(result);
            Assert.Equal(password,result);
        }
        

    }
}
