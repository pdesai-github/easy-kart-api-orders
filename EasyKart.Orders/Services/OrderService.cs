using EasyKart.Orders.Repositories;
using EasyKart.Shared.Events;
using EasyKart.Shared.Models;
using MassTransit;

namespace EasyKart.Orders.Services
{
    public class OrderService : IOrderService
    {
        IOrderRepository _orderRepository;
        IPublishEndpoint _publishEndpoint;
        public OrderService(IOrderRepository orderRepository,IPublishEndpoint publishEndpoint)
        {
            _orderRepository = orderRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task AddOrderAsync(Cart cart)
        {
            var order = new Order(cart.UserId, cart);
            OrderCreatedEvent orderCreated = new OrderCreatedEvent
            {
               CreatedAt = DateTime.Now,
               Order = order
            };
            await _orderRepository.AddOrderAsync(order);
            await _publishEndpoint.Publish(orderCreated);
        }

        public async Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            var orders = await _orderRepository.GetOrdersByUserId(userId);
            //sort orders by date
            return orders.OrderByDescending(o => o.CreatedDate).ToList();
        }
    }
}
