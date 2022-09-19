using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class RequestPasswordResetModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
