using MongoDB.Bson.Serialization.Attributes;

namespace Data.Models
{
    public class AccountInfosModel
    {
        [BsonId]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;
        public string EncryptedPasswordIV { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
