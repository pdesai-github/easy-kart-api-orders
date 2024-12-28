using EasyKart.Shared.Models;

namespace EasyKart.Orders.Services
{
    public interface IOrderService
    {
        Task AddOrderAsync(Cart cart);
        Task<List<Order>> GetOrdersByUserId(Guid userId);
    }
}
