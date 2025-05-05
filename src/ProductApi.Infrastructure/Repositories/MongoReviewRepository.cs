using MongoDB.Driver;
using Microsoft.Extensions.Options;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Interfaces;
using ProductApi.Infrastructure.Configuration;

namespace ProductApi.Infrastructure.Repositories
{
    public class MongoReviewRepository : IReviewRepository
    {
        private readonly IMongoCollection<Review> _collection;

        public MongoReviewRepository(IOptions<MongoDbSettings> options)
        {
            var settings = options.Value;
            var dbName = settings.DatabaseName;

            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(dbName);
            _collection = db.GetCollection<Review>("Reviews");
        }

        public async Task AddAsync(Review review) =>
            await _collection.InsertOneAsync(review);

        public async Task<IEnumerable<Review>> GetByProductIdAsync(string productId) =>
            await _collection.Find(r => r.ProductId == productId).ToListAsync();
    }
}
