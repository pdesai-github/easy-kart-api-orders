using EasyKart.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyKart.Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        OrdersDBContext _context;
        public OrderRepository(OrdersDBContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }

        // Get Orders by user id
        public async Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            List<Order> orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ToListAsync();
            return orders;
        }
    }
}
