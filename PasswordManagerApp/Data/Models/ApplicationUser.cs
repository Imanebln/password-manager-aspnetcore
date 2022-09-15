using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public RefreshTokenModel? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public string EncryptedKey { get; set; } = string.Empty;
        public string EncryptedKeyIV { get; set; } = string.Empty;
    }
}
