using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class RefreshTokenModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ExpirationDate { get; set; }
    }
}
