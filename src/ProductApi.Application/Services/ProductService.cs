using ProductApi.Domain.Entities;
using ProductApi.Domain.Interfaces;

namespace ProductApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Product>> GetAllAsync() => _repository.GetAllAsync();

    public Task<Product?> GetByIdAsync(string id) => _repository.GetByIdAsync(id);

    public Task<Product> CreateAsync(Product product) => _repository.AddAsync(product);

    public Task UpdateAsync(Product product) => _repository.UpdateAsync(product);

    public Task DeleteAsync(string id) => _repository.DeleteAsync(id);
}
