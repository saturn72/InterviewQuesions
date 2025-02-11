using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.Api.Controllers;

[Route("/api/store")]
[Authorize]
public class StoreController : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> GetStoreInfoAsync()
    {
        throw new NotImplementedException("do something async...");
    }

    [HttpGet("open-hours")]
    [AllowAnonymous]
    public Task<IActionResult> GetOpenHoursAsync()
    {
        throw new NotImplementedException("do something async...");
    }
}