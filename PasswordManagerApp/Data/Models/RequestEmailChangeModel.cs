using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class RequestEmailChangeModel
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [EmailAddress]
        public string NewEmail { get; set; } = String.Empty;
    }
}
