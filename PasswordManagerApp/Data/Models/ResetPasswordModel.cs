using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class ResetPasswordModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("NewPassword",ErrorMessage = "Passwords do not match!")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
