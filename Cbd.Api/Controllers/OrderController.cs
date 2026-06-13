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
    /// <summary>
    /// Přijme nové objednávky, uloží je do úložiště a zařadí do fronty pro další agregaci.
    /// </summary>
    [HttpPost("Accept", Name = "AcceptNewOrders")]
    [ProducesResponseType(200)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
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

    /// <summary>
    /// Vrátí všechny dosud přijaté objednávky jako pole
    /// </summary>
    [HttpGet("GetAll", Name = "GetAllOrders")]
    [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
    [ProducesResponseType(429)]
    [ProducesResponseType(500)]
    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken)
        => await ordersRepository.GetAll(cancellationToken);
}