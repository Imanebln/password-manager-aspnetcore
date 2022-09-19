namespace AuthenticationService.Models
{
    public class OTPLoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
    }
}
