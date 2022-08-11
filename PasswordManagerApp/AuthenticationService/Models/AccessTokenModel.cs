using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Models
{
    public class AccessTokenModel
    {
        public AccessTokenModel(string accessToken, DateTime creationDate, DateTime expirationDate, IEnumerable<string> roles)
        {
            AccessToken = accessToken;
            CreationDate = creationDate;
            ExpirationDate = expirationDate;
            Roles = roles;
        }

        public string AccessToken { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
