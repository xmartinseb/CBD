using Cbd.Api.Data;
using Cbd.Api.Models;
using Cbd.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cbd.Api.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("default")]
public sealed class OrderController(CreatedOrdersChannel createdOrdersQueue, IOrdersRepository ordersRepository, ILogger<OrderController> logger)
    : ControllerBase
{
    [HttpPost("Accept", Name = "AcceptNewOrders")]
    public async Task<IActionResult> AcceptAsync([FromBody] Order[] newOrders, CancellationToken cancellationToken)
    {
        foreach (var order in newOrders)
        {
            try
            {
                await ordersRepository.AddAsync(order, cancellationToken);
                await createdOrdersQueue.EnqueueOrderAsync(new OrderCreated(order, DateTime.UtcNow), cancellationToken);
                logger.LogInformation("New order registered: Product {p}, Quantity={q}", order.ProductId, order.Quantity);
            }
            // TODO: zachytit pouze chyby souvisejici s jednim orderem (povolit partial failure).
            // Např. při výpadku databáze nechat spadnout celý http request (full failure)
            catch (Exception ex)
            {
                logger.LogError(ex, "Storing an order failed");
            }
        }

        return Ok();
    }

    [HttpGet("GetAll", Name = "GetAllOrders")]
    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken)
        => await ordersRepository.GetAll(cancellationToken);
}