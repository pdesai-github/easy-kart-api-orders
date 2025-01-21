
using EasyKart.Orders.Consumers;
using EasyKart.Orders.Repositories;
using EasyKart.Orders.Services;
using EasyKart.Shared.Commands;
using EasyKart.Shared.Events;
using MassTransit;

namespace EasyKart.Orders
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowCors", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            builder.Services.AddMassTransit((x) => 
            {
                x.AddConsumer<UpdateOrderCommandConsumer>();
                x.UsingAzureServiceBus((context, config) => 
                {
                    config.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));

                  
                    config.Message<OrderCreatedEvent>(configTopology =>
                    {
                        configTopology.SetEntityName("ordercreated");
                    });
                    config.Message<UpdateOrderCommand>(configTopology =>
                    {
                        configTopology.SetEntityName("ordercreated");
                    });

                    config.SubscriptionEndpoint("UpdateOrderCommandConsumerSubscription", "ordercreated", e =>
                    {
                        e.ConfigureConsumer<UpdateOrderCommandConsumer>(context);
                    });
                });
            });

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCors("AllowCors");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
