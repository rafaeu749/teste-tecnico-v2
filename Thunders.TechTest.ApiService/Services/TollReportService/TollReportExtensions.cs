using System.Text.Json;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Services.TollReportService;

public static class TollReportExtensions
{
    public static void SerializeData(this TollReport report)
    {
        if (report.Data == null)
        {
            report.SerializedData = null;
            return;
        }

        report.SerializedData = JsonSerializer.Serialize(report.Data, report.Data.GetType());
    }

    public static void DeserializeData(this TollReport report)
    {
        if (string.IsNullOrEmpty(report.SerializedData)) return;

        report.Data = report.ReportType switch
        {
            ReportType.CityRevenuePerHour => JsonSerializer.Deserialize<CityRevenuePerHourResult>(report.SerializedData),
            ReportType.TopTollPlazasPerMonth => JsonSerializer.Deserialize<TopTollPlazasPerMonthResult>(report.SerializedData),
            ReportType.VehicleTypePerPlaza => JsonSerializer.Deserialize<VehicleTypePerPlazaResult>(report.SerializedData),
            _ => null
        };
    }


}
