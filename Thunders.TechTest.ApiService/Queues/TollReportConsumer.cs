using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rebus.Handlers;
using Thunders.TechTest.ApiService.Database;
using Thunders.TechTest.ApiService.Services.TollReportService;

namespace Thunders.TechTest.ApiService.Queues;

public class TollReportConsumer 
    : IHandleMessages<ReportCityRevenuePerHourMessage>,
    IHandleMessages<ReportTopTollPlazasPerMonthMessage>,
    IHandleMessages<ReportVehicleTypePerPlazaMessage>
{
    private readonly ApplicationDbContext _context;

    public TollReportConsumer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(ReportCityRevenuePerHourMessage message)
    {
        var plazaIdList = _context.TollPlazas
            .Where(p => p.City == message.City && p.State == message.State)
            .Select(p => p.Id);

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var result = new CityRevenuePerHourResult
        {
            City = message.City,
            RevenuePerHour = _context.TollRegisters
                .Where(r => plazaIdList.Contains(r.TollPlazaId)
                    && r.RegisteredAt >= today && r.RegisteredAt < tomorrow)
                .GroupBy(r => new { r.RegisteredAt.Hour })
                .Select(g => new RevenuePerHourItem(g.Key.Hour, g.Sum(r => r.AmountPaid)))
                .ToList()
        };

        await UpdateReportData(message.ReportId, result);
    }

    public async Task Handle(ReportTopTollPlazasPerMonthMessage message)
    {
        var list = _context.Database
            .SqlQueryRaw<TopTollPlazaItem>(
                @"SELECT p.Id AS PlazaId, p.Name AS PlazaName, SUM(r.AmountPaid) AS TotalRevenue
                FROM TollRegisters r
                JOIN TollPlazas p ON r.TollPlazaId = p.Id
                WHERE MONTH(r.RegisteredAt) = @month AND YEAR(r.RegisteredAt) = @year
                GROUP BY p.Id, p.Name",
                new SqlParameter("@month", message.Date.Month),
                new SqlParameter("@year", message.Date.Year)
            )
            .OrderByDescending(i => i.TotalRevenue)
            .Take(message.PlazaCount)
            .ToList();

        var result = new TopTollPlazasPerMonthResult
        {
            Month = message.Date,
            TopTollPlazas = list
        };

        await UpdateReportData(message.ReportId, result);
    }

    public async Task Handle(ReportVehicleTypePerPlazaMessage message)
    {
        var list = _context.TollRegisters
            .Where(r => r.TollPlazaId == message.PlazaId)
            .GroupBy(r => r.VehicleType)
            .Select(g => new VehicleTypePerPlazaItem(
                g.Key,
                g.Count()
            ))
            .ToList();

        var result = new VehicleTypePerPlazaResult
        {
            PlazaId = message.PlazaId,
            VehicleTypePerPlaza = list
        };

        await UpdateReportData(message.ReportId, result);
    }

    private async Task UpdateReportData(Guid id, Object data)
    {
        var report = _context.TollReports.Where(r => r.Id == id).First();

        report.ProcessedAt = DateTime.UtcNow;
        report.Data = data;

        report.SerializeData();

        _context.TollReports.Update(report);
        await _context.SaveChangesAsync();
    }
}

public record ReportCityRevenuePerHourMessage(Guid ReportId, string City, string State);
public record ReportTopTollPlazasPerMonthMessage(Guid ReportId, DateTime Date, int PlazaCount);
public record ReportVehicleTypePerPlazaMessage(Guid ReportId, Guid PlazaId);