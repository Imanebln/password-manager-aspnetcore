using MongoDB.Bson.Serialization.Attributes;

namespace Data.Models
{
    public class UserDataModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public IEnumerable<AccountInfosModel>? AccountInfos { get; set; }
    }
}
