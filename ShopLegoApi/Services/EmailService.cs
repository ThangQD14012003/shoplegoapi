using System.Text.Json;
using RabbitMQ.Client;
using ShopLegoApi.Datas;

namespace ShopLegoApi.Services
{
    public class EmailService : IEmailService // đây là Producer 
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            LegoDataContext context,
            IConfiguration config,
            ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendOrderConfirmationAsync(int orderId)
        {
            try
            {
                var rabbitSection = _config.GetSection("RabbitMq"); // đọc config của rabbitMQ
                string hostName = rabbitSection.GetValue<string>("HostName") ?? "localhost";
                string userName = rabbitSection.GetValue<string>("UserName") ?? "guest";
                string password = rabbitSection.GetValue<string>("Password") ?? "guest";
                string queueName = rabbitSection.GetValue<string>("QueueName") ?? "email_queue";

                var factory = new ConnectionFactory()
                {
                    HostName = hostName,
                    UserName = userName,
                    Password = password
                };

                await using var connection = await factory.CreateConnectionAsync(); // tạo connection 
                await using var channel = await connection.CreateChannelAsync(); // tạo chanel, đường giao tiếp để gửi message

                await channel.QueueDeclareAsync( // khai báo queue
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var message = new EmailQueueMessage // tạo message
                {
                    OrderId = orderId,
                    EnqueuedAt = DateTime.UtcNow
                };

                var body = JsonSerializer.SerializeToUtf8Bytes(message);

                var properties = new BasicProperties
                {
                    Persistent = true
                };

                await channel.BasicPublishAsync(
                    exchange: string.Empty,
                    routingKey: queueName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body); // publish vào queue

                _logger.LogInformation(
                    "Published email queue message for Order {OrderId} to queue {QueueName}.",
                    orderId, queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to publish email queue message for Order {OrderId}.",
                    orderId);
            }
        }
    }
}
