using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Item.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResourceController : ControllerBase
{
    //[Authorize]
    [HttpGet]
    [Route("verify")]
    public IActionResult Verify()
    {
        return Ok();
    }
}
