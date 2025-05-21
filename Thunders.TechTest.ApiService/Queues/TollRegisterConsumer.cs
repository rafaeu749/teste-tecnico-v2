using Rebus.Handlers;
using Thunders.TechTest.ApiService.Database;
using Thunders.TechTest.ApiService.Models;

namespace Thunders.TechTest.ApiService.Queues;

public class TollRegisterConsumer : IHandleMessages<TollRegisterMessage>
{
    private readonly ApplicationDbContext _context;

    public TollRegisterConsumer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(TollRegisterMessage message)
    {
        var registerData = new TollRegister
        {
            Id = message.Id,
            CreatedAt = DateTime.UtcNow,
            RegisteredAt = message.RegisteredAt,
            AmountPaid = message.AmountPaid,
            VehicleType = message.VehicleType,
            TollPlazaId = message.TollPlazaId
        };

        _context.TollRegisters.Add(registerData);
        await _context.SaveChangesAsync();
    }
}

public record TollRegisterMessage(
    Guid Id,
    DateTime RegisteredAt, 
    decimal AmountPaid, 
    VehicleType VehicleType, 
    Guid TollPlazaId
);