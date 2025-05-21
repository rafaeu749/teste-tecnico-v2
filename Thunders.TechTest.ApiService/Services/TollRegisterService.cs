using Thunders.TechTest.ApiService.Validators;
using FluentValidation;
using Thunders.TechTest.ApiService.Queues;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Services;
public interface ITollRegisterService
{
    Task ProcessRegisterData(TollRegisterMessage message);
}

public class TollRegisterService : ITollRegisterService
{
    private readonly IMessageSender _messageSender;

    public TollRegisterService(IMessageSender messageSender)
    {
        _messageSender = messageSender;
    }

    public async Task ProcessRegisterData(TollRegisterMessage message)
    {
        var validator = new TollRegisterMessageValidator();
        var validationResult = validator.Validate(message);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        await _messageSender.SendLocal(message);
    }

}
