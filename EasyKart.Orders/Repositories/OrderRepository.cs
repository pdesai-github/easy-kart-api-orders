using EasyKart.Shared.Models;
using Microsoft.Azure.Cosmos;

namespace EasyKart.Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        IConfiguration _configuration;
        private readonly string _cosmosEndpoint;
        private readonly string _cosmosKey;
        private readonly string _databaseId;
        private readonly string _containerId;
        private readonly string _partitionKey;

        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public OrderRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosEndpoint = _configuration["CosmosDB:endpoint"];
            _cosmosKey = _configuration["CosmosDB:authKey"];
            _databaseId = _configuration["CosmosDB:databaseId"];
            _containerId = _configuration["CosmosDB:containerId"];
            _partitionKey = _configuration["CosmosDB:partitionKey"];

            _cosmosClient = new CosmosClient(_cosmosEndpoint, _cosmosKey);
            _container = _cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task AddOrderAsync(Order order)
        {
            try
            {
           
                await _container.CreateItemAsync(order, new PartitionKey(order.UserId.ToString()));
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB exceptions (e.g., for network issues, timeouts, etc.)
                throw new Exception($"Error adding order to Cosmos DB: {ex.Message}", ex);
            }

        }

        // Get Orders by user id
        public async Task<List<Order>> GetOrdersByUserId(Guid userId)
        {

            try
            {
                var query = _container.GetItemQueryIterator<Order>(
                    new QueryDefinition("SELECT * FROM c WHERE c.userId = @userId")
                    .WithParameter("@userId", userId.ToString())
                );

                var orders = new List<Order>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    orders.AddRange(response.Resource);
                }

                return orders;
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB exceptions (e.g., for network issues, timeouts, etc.)
                throw new Exception($"Error fetching orders from Cosmos DB: {ex.Message}", ex);
            }
        }

        // update order
        public async Task UpdateOrderAsync(Order order)
        {
            try
            {
                var orderToUpdate = new
                {
                    id = order.Id.ToString(),
                    userId = order.UserId.ToString(),
                    items = order.Items,
                    price = order.Price,
                    createdDate = order.CreatedDate,
                    status = order.Status
                };

                var response = await _container.UpsertItemAsync(orderToUpdate, new PartitionKey(order.UserId.ToString()));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Failed to update order. Status code: {response.StatusCode}");
                }
            }
            catch (CosmosException ex)
            {
                // Handle Cosmos DB exceptions (e.g., for network issues, timeouts, etc.)
                throw new Exception($"Error updating order in Cosmos DB: {ex.Message}", ex);
            }
        }
    }
}

