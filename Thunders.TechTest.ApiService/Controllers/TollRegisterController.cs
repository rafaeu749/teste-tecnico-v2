using Microsoft.AspNetCore.Mvc;
using Thunders.TechTest.ApiService.Queues;
using Thunders.TechTest.ApiService.Services;

namespace Thunders.TechTest.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TollRegisterController : ControllerBase
{
    private readonly ITollRegisterService _tollRegisterService;

    public TollRegisterController(ITollRegisterService tollUsageService)
    {
        _tollRegisterService = tollUsageService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTollRegister([FromBody] TollRegisterMessage registerData)
    {
        if (registerData == null)
        {
            return BadRequest("No data provided");
        }

        try
        {
            await _tollRegisterService.ProcessRegisterData(registerData);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing the data: {ex.Message}");
        }
    }
}
