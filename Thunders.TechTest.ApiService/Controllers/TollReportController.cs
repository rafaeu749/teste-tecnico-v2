using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.Database;
using Thunders.TechTest.ApiService.Services.TollReportService;

namespace Thunders.TechTest.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TollReportController : ControllerBase
{
    public ITollReportService _tollReportService;

    public TollReportController(ITollReportService tollReportService)
    {
        _tollReportService = tollReportService;
    }

    [HttpGet("state/{state}/city/{city}/revenue")]
    public async Task<IActionResult> CreateCityRevenuePerHour(string city, string state)
    {
        var reportId = await _tollReportService.CreateCityRevenuePerHour(city, state);
        return Created(nameof(CreateCityRevenuePerHour), reportId);
    }

    [HttpGet("plazas/month/{month}/top/{quantity}")]
    public async Task<IActionResult> CreateTopTollPlazasPerMonth(int month, int quantity)
    {
        var monthDate = new DateTime(DateTime.Now.Year, month, 1);
        var reportId = await _tollReportService.CreateTopTollPlazasPerMonth(monthDate, quantity);
        return Created(nameof(CreateTopTollPlazasPerMonth), reportId);
    }

    [HttpGet("plaza/{plazaId}/vehicletypes")]
    public async Task<IActionResult> CreateVehicleTypePerPlaza(Guid plazaId)
    {
        var reportId = await _tollReportService.CreateVehicleTypePerPlaza(plazaId);
        return Created(nameof(CreateVehicleTypePerPlaza), reportId);
    }

    [HttpGet("{reportId}")]
    public async Task<IActionResult> GetTollReport(Guid reportId)
    {
        var report = await _tollReportService.GetTollReport(reportId);
        if (report == null)
        {
            return NotFound();
        }
        if (report.ProcessedAt == null || report.SerializedData == null)
        {
            return BadRequest("Report is still being processed");
        }
        return Ok(report.Data);
    }

    // TODO - Remove this endpoint
    [HttpGet()]
    public async Task<IActionResult> GetTollReports([FromServices] ApplicationDbContext context)
    {
        var reports = await context.TollReports
            .Select(r => new
            {
                r.Id,
                r.ProcessedAt,
                ReportType = r.ReportType.ToString(),
            })
            .ToListAsync();
        return Ok(reports);
    }
}
