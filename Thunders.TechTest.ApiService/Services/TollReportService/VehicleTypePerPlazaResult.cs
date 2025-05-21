using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Services.TollReportService;

public class VehicleTypePerPlazaResult
{
    public required Guid PlazaId { get; set; }
    public List<VehicleTypePerPlazaItem> VehicleTypePerPlaza { get; set; } = [];
}

public record VehicleTypePerPlazaItem(VehicleType VehicleType, int Count);