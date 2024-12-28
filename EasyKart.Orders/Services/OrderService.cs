using EasyKart.Orders.Repositories;
using EasyKart.Shared.Models;

namespace EasyKart.Orders.Services
{
    public class OrderService : IOrderService
    {
        IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task AddOrderAsync(Cart cart)
        {
            var order = new Order(cart.UserId, cart);
            await _orderRepository.AddOrderAsync(order);
        }

        public async Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            return await _orderRepository.GetOrdersByUserId(userId);
        }
    }
}
