using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class AccountInfosModel
    {
        public string Email { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
    }
}
