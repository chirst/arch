using Microsoft.AspNetCore.Mvc;

namespace Arch.Controllers;

[Controller]
[Route("[controller]")]
public class FooController : ControllerBase
{
    [HttpGet(Name = "GetFoo")]
    public IActionResult Get()
    {
        return Ok("wat");
    }
}
