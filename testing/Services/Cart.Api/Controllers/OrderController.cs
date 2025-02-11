using Cart.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.Api.Controllers;

[Route("/api/order")]
[Authorize]
public class OrderController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> GetOrdersAsync([FromQuery] int? offset = 0, [FromQuery] int? pageSize = 15)
    {
        throw new NotImplementedException("do something async...");
    }

    [HttpGet("{orderId}")]
    public Task<IActionResult> GetOrderByIdAsync(int orderId)
    {
        throw new NotImplementedException("do something async...");
    }

    [HttpPost]
    public Task<IActionResult> CreateOrderByIdAsync([FromBody] OrderModel model)
    {
        throw new NotImplementedException("do something async...");
    }
}
