using EasyKart.Shared.Models;

namespace EasyKart.Orders.Repositories
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<List<Order>> GetOrdersByUserId(Guid userId);
    }
}
