using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;

namespace EasyKart.Orders.CircuitBreaker
{
    public class DatabaseRetryPolicy
    {
        public static AsyncRetryPolicy GetRetryPolicy()
        {
            return Policy.Handle<SqlException>()
                .WaitAndRetryAsync(
                retryCount: 5, // Number of retry attempts
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)), // Exponential backoff
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} encountered an error: {exception.Message}. Waiting {timeSpan} before next retry.");
                });
        }
    }
}
