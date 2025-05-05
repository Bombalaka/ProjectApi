using ProductApi.Domain.Entities;

namespace ProductApi.Domain.Interfaces
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<IEnumerable<Review>> GetByProductIdAsync(string productId);
    }
}
