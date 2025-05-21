namespace Thunders.TechTest.ApiService.Services.TollReportService;

public class CityRevenuePerHourResult
{
    public required string City { get; set; }
    public List<RevenuePerHourItem> RevenuePerHour { get; set; } = new();
}

public record RevenuePerHourItem(int Hour, decimal Revenue);
