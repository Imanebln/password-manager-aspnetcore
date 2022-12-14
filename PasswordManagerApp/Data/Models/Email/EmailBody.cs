namespace Data.Models.Email
{
    public class EmailBody
    {
        public EmailType Type { get; set; } = EmailType.info;
        public string? Title { get; set; }
        public string? Message { get; set; }
        public string? RefreshToken { get; set; }
        public IList<Button> Buttons { get; set; } = new List<Button>();
    }
    public enum EmailType
    {
        success,
        info,
        warning,
        danger,
    }
    public class Button
    {
        public string? Link { get; set; }
        public string? Text { get; set; }
    }
}
