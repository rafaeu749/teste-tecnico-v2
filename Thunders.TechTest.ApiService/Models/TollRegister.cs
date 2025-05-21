namespace Thunders.TechTest.ApiService.Models;

public class TollRegister
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public required DateTime RegisteredAt { get; set; }
    public required decimal AmountPaid { get; set; }
    public required VehicleType VehicleType { get; set; }
    public required Guid TollPlazaId { get; set; }
}

public enum VehicleType
{
    Motorcycle = 1,
    Car,
    Truck
}
