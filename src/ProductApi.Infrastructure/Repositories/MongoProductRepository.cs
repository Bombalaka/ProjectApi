using MongoDB.Driver;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using ProductApi.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ProductApi.Infrastructure.Repositories;

public class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

    public MongoProductRepository(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        if (string.IsNullOrEmpty(settings.ConnectionString))
            throw new ArgumentException("MongoDB connection string is missing.");

        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _collection = database.GetCollection<Product>("Products");
    }


    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetByIdAsync(string id) =>
        await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<Product> AddAsync(Product product)
    {
        // Generate a new ID using MongoDB's ObjectId
        product.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        await _collection.InsertOneAsync(product);
        return product;
    }

    public async Task UpdateAsync(Product product) =>
        await _collection.ReplaceOneAsync(p => p.Id == product.Id, product);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(p => p.Id == id);
}
