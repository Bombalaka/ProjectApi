using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Domain.Entities
{
    public class Review
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string? ProductId { get; set; }

        public int Stars { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
