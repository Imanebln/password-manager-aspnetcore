using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class UserDto
    {
        [Required]
        [MinLength(8)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}
