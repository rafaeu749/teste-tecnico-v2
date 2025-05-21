namespace Thunders.TechTest.ApiService.Services.TollReportService;

public class TopTollPlazasPerMonthResult
{
    public required DateTime Month { get; set; }
    public List<TopTollPlazaItem> TopTollPlazas { get; set; } = new();
}

public record TopTollPlazaItem(
    Guid PlazaId,
    string PlazaName,
    decimal TotalRevenue
);
