namespace Data.Models
{
    public class RefreshTokenModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime ExpirationDate { get; set; }
    }
}
