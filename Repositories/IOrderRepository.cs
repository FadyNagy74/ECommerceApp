using E_CommerceApp.Models;

namespace E_CommerceApp.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        public Task<Order?> GetOrderWithItems(string orderId);
        public Task<List<Order>> GetOrdersStatusSorted(bool asc, int pageNumber);
        public Task<List<Order>> GetOrdersDateSorted(bool asc, int pageNumber);

    }
}
