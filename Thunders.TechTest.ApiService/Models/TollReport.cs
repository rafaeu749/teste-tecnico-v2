using System.ComponentModel.DataAnnotations.Schema;

namespace Thunders.TechTest.ApiService.Models;

public class TollReport
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public ReportType ReportType { get; set; }
    public Object? Data { get; set; }
    public string? SerializedData { get; set; }
}

public enum ReportType
{
    CityRevenuePerHour,
    TopTollPlazasPerMonth,
    VehicleTypePerPlaza
}
