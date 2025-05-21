using Thunders.TechTest.ApiService.Database;
using Thunders.TechTest.ApiService.Models;
using Thunders.TechTest.ApiService.Queues;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Services.TollReportService;
public interface ITollReportService
{
    public Task<Guid> CreateCityRevenuePerHour(string city, string state);
    public Task<Guid> CreateTopTollPlazasPerMonth(DateTime date, int plazaCount);
    public Task<Guid> CreateVehicleTypePerPlaza(Guid plazaId);
    public Task<TollReport>? GetTollReport(Guid reportId);
}

public class TollReportService : ITollReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IMessageSender _messageSender;
    public TollReportService(ApplicationDbContext context, IMessageSender messageSender)
    {
        _context = context;
        _messageSender = messageSender;
    }

    public Task<Guid> CreateCityRevenuePerHour(string city, string state)
    {
        var id = CreateNew(ReportType.CityRevenuePerHour);

        _messageSender.SendLocal(new ReportCityRevenuePerHourMessage(id, city, state));

        return Task.FromResult(id);
    }

    public Task<Guid> CreateTopTollPlazasPerMonth(DateTime date, int plazaCount)
    {
        var id = CreateNew(ReportType.TopTollPlazasPerMonth);

        _messageSender.SendLocal(new ReportTopTollPlazasPerMonthMessage(id, date, plazaCount));

        return Task.FromResult(id);
    }

    public Task<Guid> CreateVehicleTypePerPlaza(Guid plazaId)
    {
        var id = CreateNew(ReportType.VehicleTypePerPlaza);

        _messageSender.SendLocal(new ReportVehicleTypePerPlazaMessage(id, plazaId));

        return Task.FromResult(id);
    }

    public Task<TollReport>? GetTollReport(Guid reportId)
    {
        var report = _context.TollReports.Where(r => r.Id == reportId).FirstOrDefault();
        if (report == null)
        {
            return null;
        }

        report.DeserializeData();
        return Task.FromResult(report);
    }

    private Guid CreateNew(ReportType reportType)
    {
        var report = new TollReport
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ReportType = reportType
        };

        report.SerializeData();

        _context.TollReports.Add(report);
        _context.SaveChanges();

        return report.Id;
    }
}