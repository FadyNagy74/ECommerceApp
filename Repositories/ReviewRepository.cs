using E_CommerceApp.DTOs;
using E_CommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceApp.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<List<ShowReviewDTO>?> GetProductReviews(string productId) {
            var reviews = await _dbSet.AsNoTracking()
                .Include(review => review.User)
                .Where(review => review.ProductId == productId)
                .Select(review => new ShowReviewDTO 
                {
                    ReviewOwner = review.User.UserName,
                    Review = review.Description,
                    RateValue = review.RateValue,
                    IsEdited = review.IsEdited
                })
                .ToListAsync();
            return reviews;
        }
    }
}
