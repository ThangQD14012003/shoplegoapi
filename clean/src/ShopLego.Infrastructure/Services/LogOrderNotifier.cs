using Microsoft.Extensions.Logging;
using ShopLego.Application;
namespace ShopLego.Infrastructure.Services;

public sealed class LogOrderNotifier(ILogger<LogOrderNotifier> logger) : IOrderNotifier
{ public Task OrderPlacedAsync(int orderId, CancellationToken ct) { logger.LogInformation("Order {OrderId} placed; notification integration pending.", orderId); return Task.CompletedTask; } }
