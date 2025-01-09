using EasyKart.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Polly.Retry;

namespace EasyKart.Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        OrdersDBContext _context;
        private readonly AsyncRetryPolicy _retryPolicy;
        public OrderRepository(OrdersDBContext context, AsyncRetryPolicy retryPolicy)
        {
            _context = context;
            _retryPolicy = retryPolicy;
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
           
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.Items)
                    .ToListAsync();
            });
        }

        // update order
        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
