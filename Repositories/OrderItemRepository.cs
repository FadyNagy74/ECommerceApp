using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context) { }
    }
}
