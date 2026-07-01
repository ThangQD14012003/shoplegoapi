using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShopLegoApi.Datas;

namespace ShopLegoApi.Services
{
    public class EmailBackgroundService : BackgroundService //Consumer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _config;
        private readonly ILogger<EmailBackgroundService> _logger;
        private global::RabbitMQ.Client.IConnection? _connection;
        private global::RabbitMQ.Client.IChannel? _channel;

        public EmailBackgroundService(
            IServiceProvider serviceProvider,
            IConfiguration config,
            ILogger<EmailBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var rabbitSection = _config.GetSection("RabbitMq");
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

                _connection = await factory.CreateConnectionAsync(stoppingToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

                await _channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    try
                    {
                        var body = eventArgs.Body.ToArray();
                        var message = JsonSerializer.Deserialize<EmailQueueMessage>(body);

                        if (message != null)
                        {
                            await ProcessMessageAsync(message);
                            await _channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                        }
                        else
                        {
                            _logger.LogWarning("Received invalid email queue message.");
                            await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Email consumer failed processing a message.");
                        await _channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                };

                await _channel.BasicConsumeAsync(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("RabbitMQ email consumer started on queue {QueueName}.", queueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize RabbitMQ connection or channel.");
            }

            // Keep the background service alive until cancelled
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        private async Task ProcessMessageAsync(EmailQueueMessage message)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<LegoDataContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmailBackgroundService>>();

            var order = await context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == message.OrderId);

            if (order == null)
            {
                logger.LogWarning("Order {OrderId} not found when consuming email queue.", message.OrderId);
                return;
            }

            var managerEmailSetting = await context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "ManagerEmail");
            var accountantEmailSetting = await context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "AccountantEmail");

            string managerEmail = managerEmailSetting?.Value ?? "manager@example.com";
            string accountantEmail = accountantEmailSetting?.Value ?? "accountant@example.com";
            string customerEmail = order.User?.Email ?? string.Empty;

            var recipients = new List<(string Email, string Role)>
            {
                (customerEmail, "Customer"),
                (managerEmail, "Manager"),
                (accountantEmail, "Accountant")
            }
            .Where(r => !string.IsNullOrWhiteSpace(r.Email))
            .ToList();

            if (!recipients.Any())
            {
                logger.LogWarning("No recipients found for order {OrderId} during email consumption.", message.OrderId);
                return;
            }

            string htmlBody = BuildOrderEmailTemplate(order);
            string subject = $"[Lego Shop] Xác Nhận Đơn Hàng Thành Công #{order.Id}";

            var smtpSection = _config.GetSection("SmtpSettings");
            string host = smtpSection.GetValue<string>("Host") ?? "smtp.gmail.com";
            int port = smtpSection.GetValue<int>("Port", 587);
            string username = smtpSection.GetValue<string>("Username") ?? string.Empty;
            string password = smtpSection.GetValue<string>("Password") ?? string.Empty;
            bool enableSsl = smtpSection.GetValue<bool>("EnableSsl", true);
            string fromEmail = smtpSection.GetValue<string>("FromEmail") ?? username;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                logger.LogError("SMTP credentials not configured. Email cannot be sent for Order {OrderId}.", order.Id);
                await LogEmailAsync(context, order.Id, recipients, subject, false);
                return;
            }

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            foreach (var recipient in recipients)
            {
                try
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, "Lego Shop"),
                        Subject = subject,
                        Body = htmlBody,
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        SubjectEncoding = Encoding.UTF8
                    };
                    mailMessage.To.Add(recipient.Email);

                    await client.SendMailAsync(mailMessage);
                    await LogEmailAsync(context, order.Id, recipient.Email, subject, true);
                    logger.LogInformation("Email sent for Order {OrderId} to {Recipient}.", order.Id, recipient.Email);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send email for Order {OrderId} to {Recipient}.", order.Id, recipient.Email);
                    await LogEmailAsync(context, order.Id, recipient.Email, subject, false);
                }
            }
        }

        private static async Task LogEmailAsync(LegoDataContext context, int orderId, string receiverEmail, string subject, bool status)
        {
            var log = new EmailLog
            {
                OrderId = orderId,
                ReceiverEmail = receiverEmail,
                Subject = subject,
                SendTime = DateTime.UtcNow,
                Status = status
            };
            context.EmailLogs.Add(log);
            await context.SaveChangesAsync();
        }

        private static async Task LogEmailAsync(LegoDataContext context, int orderId, List<(string Email, string Role)> recipients, string subject, bool status)
        {
            foreach (var recipient in recipients)
            {
                var log = new EmailLog
                {
                    OrderId = orderId,
                    ReceiverEmail = recipient.Email,
                    Subject = subject,
                    SendTime = DateTime.UtcNow,
                    Status = status
                };
                context.EmailLogs.Add(log);
            }
            await context.SaveChangesAsync();
        }

        private string BuildOrderEmailTemplate(Order order)
        {
            var sb = new StringBuilder();
            sb.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <style>
        body { font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333333; margin: 0; padding: 0; background-color: #f4f6f8; }
        .container { max-width: 600px; margin: 20px auto; background: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }
        .header { background-color: #1e3a8a; color: #ffffff; padding: 30px; text-align: center; }
        .header h1 { margin: 0; font-size: 24px; font-weight: 600; letter-spacing: 0.5px; }
        .content { padding: 30px; }
        .order-info { margin-bottom: 25px; padding-bottom: 15px; border-bottom: 1px solid #e5e7eb; }
        .order-info p { margin: 5px 0; font-size: 14px; }
        .table { width: 100%; border-collapse: collapse; margin-top: 15px; }
        .table th { background-color: #f3f4f6; color: #374151; font-weight: 600; text-align: left; padding: 12px; border-bottom: 2px solid #e5e7eb; font-size: 14px; }
        .table td { padding: 12px; border-bottom: 1px solid #e5e7eb; font-size: 14px; }
        .total-row { font-weight: bold; background-color: #fafafa; }
        .footer { background-color: #f9fafb; padding: 20px; text-align: center; font-size: 12px; color: #6b7280; border-top: 1px solid #e5e7eb; }
        .button { display: inline-block; padding: 10px 20px; margin-top: 20px; background-color: #1e3a8a; color: #ffffff !important; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 14px; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Xác Nhận Đơn Hàng Thành Công</h1>
        </div>
        <div class='content'>
            <p>Xin chào,</p>
            <p>Đơn hàng của khách hàng <strong>" + (order.User?.FullName ?? "Khách hàng") + @"</strong> đã được đặt thành công trên hệ thống <strong>Lego Shop</strong>.</p>
            
            <div class='order-info'>
                <p><strong>Mã đơn hàng:</strong> #" + order.Id + @"</p>
                <p><strong>Ngày đặt:</strong> " + order.OrderDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm") + @"</p>
                <p><strong>Địa chỉ giao hàng:</strong> " + order.ShippingAddress + @"</p>
            </div>

            <table class='table'>
                <thead>
                    <tr>
                        <th>Sản phẩm</th>
                        <th style='text-align: center;'>SL</th>
                        <th style='text-align: right;'>Đơn giá</th>
                        <th style='text-align: right;'>Thành tiền</th>
                    </tr>
                </thead>
                <tbody>");

            foreach (var detail in order.OrderDetails)
            {
                var productName = detail.Product?.Name ?? "Sản phẩm";
                var quantity = detail.Quantity;
                var unitPrice = detail.UnitPrice;
                var subtotal = quantity * unitPrice;

                sb.Append("<tr>");
                sb.Append("<td>" + productName + "</td>");
                sb.Append("<td style='text-align: center;'>" + quantity + "</td>");
                sb.Append("<td style='text-align: right;'>" + unitPrice.ToString("N0") + " đ</td>");
                sb.Append("<td style='text-align: right;'>" + subtotal.ToString("N0") + " đ</td>");
                sb.Append("</tr>");
            }

            sb.Append(@"
                    <tr class='total-row'>
                        <td colspan='3' style='text-align: right;'>Tổng cộng:</td>
                        <td style='text-align: right; color: #b91c1c;'>" + order.TotalAmount.ToString("N0") + @" đ</td>
                    </tr>
                </tbody>
            </table>

            <div style='text-align: center;'>
                <a href='#' class='button'>Xem chi tiết đơn hàng</a>
            </div>
        </div>
        <div class='footer'>
            <p>Đây là email tự động từ hệ thống Lego Shop. Vui lòng không trả lời email này.</p>
        </div>
    </div>
</body>
</html>");

            return sb.ToString();
        }
    }
}
