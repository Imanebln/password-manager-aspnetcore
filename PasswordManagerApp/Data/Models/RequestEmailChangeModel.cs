using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class RequestEmailChangeModel
    {
        [Email]
        public string Email { get; set; } = string.Empty;
        [Email]
        public string NewEmail { get; set; } = String.Empty;
    }
}
