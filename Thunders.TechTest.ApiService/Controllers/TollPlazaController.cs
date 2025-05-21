using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Database;

namespace Thunders.TechTest.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TollPlazaController : ControllerBase
{
    public TollPlazaController()
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetTollPlazas([FromServices] ApplicationDbContext context)
    {
        return Ok(await context.TollPlazas.ToListAsync());
    }
}
