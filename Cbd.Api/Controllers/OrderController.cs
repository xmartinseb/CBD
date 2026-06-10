using Cbd.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cbd.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    [HttpPost("Accept", Name = "AcceptNewOrders")]
    public async Task Accept([FromBody] Order[] newOrders)
    {
        // TODO: 1) insert into a pseudoDB

        // TODO: 2) publish the orders to a queue for further aggregation

        // TODO: 3) return
    }
}