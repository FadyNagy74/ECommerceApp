using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }
    }
}
