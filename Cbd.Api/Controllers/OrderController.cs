using Cbd.Api.Data;
using Cbd.Api.Models;
using Cbd.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cbd.Api.Controllers;

[ApiController]
[Route("[controller]")]
[EnableRateLimiting("default")]
public sealed class OrderController(CreatedOrdersChannel createdOrdersQueue, IOrdersRepository ordersRepository)
    : ControllerBase
{
    [HttpPost("Accept", Name = "AcceptNewOrders")]
    public async Task<IActionResult> AcceptAsync([FromBody] Order[] newOrders, CancellationToken cancellationToken)
    {
        foreach (var order in newOrders)
        {
            await ordersRepository.AddAsync(order, cancellationToken);
            await createdOrdersQueue.EnqueueOrderAsync(new OrderCreated(order, DateTime.UtcNow), cancellationToken);
        }

        return Ok();
    }

    [HttpGet("GetAll", Name = "GetAllOrders")]
    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken)
        => await ordersRepository.GetAll(cancellationToken);
}