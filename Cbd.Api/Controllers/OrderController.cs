using Cbd.Api.Models;
using Cbd.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cbd.Api.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class OrderController(CreatedOrdersChannel createdOrdersQueue)
    : ControllerBase
{
    [HttpPost("Accept", Name = "AcceptNewOrders")]
    public async Task<IActionResult> AcceptAsync([FromBody] Order[] newOrders, CancellationToken cancellationToken)
    {
        // TODO: insert into a pseudoDB

        foreach (var order in newOrders)
            await createdOrdersQueue.EnqueueOrderAsync(new OrderCreated(order, DateTime.UtcNow), cancellationToken);

        return Ok();
    }
}