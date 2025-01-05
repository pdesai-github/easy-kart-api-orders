using EasyKart.Orders.Repositories;
using EasyKart.Shared.Commands;
using EasyKart.Shared.Models;
using MassTransit;

namespace EasyKart.Orders.Consumers
{
    public class UpdateOrderCommandConsumer : IConsumer<UpdateOrderCommand>
    {
        IOrderRepository _orderRepository;
        public UpdateOrderCommandConsumer(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task Consume(ConsumeContext<UpdateOrderCommand> context)
        {
            Order order = context.Message.Order;
            await _orderRepository.UpdateOrderAsync(order);
        }
    }
}
