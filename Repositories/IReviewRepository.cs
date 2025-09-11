using E_CommerceApp.DTOs;
using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        public Task<List<ShowReviewDTO>?> GetProductReviews(string productId);
    }
}
