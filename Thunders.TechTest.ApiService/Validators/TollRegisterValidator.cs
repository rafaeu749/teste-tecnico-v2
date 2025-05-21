using FluentValidation;
using Thunders.TechTest.ApiService.Queues;

namespace Thunders.TechTest.ApiService.Validators
{
    public class TollRegisterMessageValidator : AbstractValidator<TollRegisterMessage>
    {
        public TollRegisterMessageValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required");

            RuleFor(x => x.TollPlazaId)
                .NotEmpty().WithMessage("TollPlazaId is required");

            RuleFor(x => x.RegisteredAt)
                .NotEmpty().WithMessage("RegisteredAt is required")
                .Must(date => date <= DateTime.UtcNow).WithMessage("RegisteredAt cannot be in the future");

            RuleFor(x => x.VehicleType)
                .IsInEnum().WithMessage("VehicleType must be a valid enum value");

            RuleFor(x => x.AmountPaid)
                .NotEmpty().WithMessage("AmountPaid is required")
                .GreaterThan(0).WithMessage("AmountPaid must be greater than 0");
        }
    }
}
