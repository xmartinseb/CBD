using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Cbd.Api.Controllers;

[ApiController]
public sealed class DefaultController : ControllerBase
{
    [HttpGet("/", Name = "DefaultPage")]
    public IActionResult GetDefaultPage()
    {
        var message = string.Format("CBD Api is running! Version = {0}, Server UTC time = {1}", 
            Assembly.GetExecutingAssembly().GetName().Version, 
            DateTime.UtcNow);
        return Content(message);
    }
}