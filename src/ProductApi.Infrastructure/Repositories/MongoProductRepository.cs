using MongoDB.Driver;
using ProductApi.Domain.Entities;
using ProductApi.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ProductApi.Infrastructure.Repositories;

public class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

    public MongoProductRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _collection = db.GetCollection<Product>("Products");
    }

    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetByIdAsync(string id) =>
        await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<Product> AddAsync(Product product)
    {
        var max = await _collection.Find(_ => true).SortByDescending(p => p.Id).Limit(1).FirstOrDefaultAsync();
        product.Id = max?.Id + 1 ?? 1;
        await _collection.InsertOneAsync(product);
        return product;
    }

    public async Task UpdateAsync(Product product) =>
        await _collection.ReplaceOneAsync(p => p.Id == product.Id, product);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(p => p.Id == id);
}
