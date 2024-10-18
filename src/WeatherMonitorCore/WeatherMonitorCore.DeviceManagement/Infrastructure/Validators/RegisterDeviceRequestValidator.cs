using FluentValidation;
using Google.OpenLocationCode;
using WeatherMonitorCore.Contract.DeviceManagementModule;

namespace WeatherMonitorCore.DeviceManagement.Infrastructure.Validators;
internal class RegisterDeviceRequestValidator : AbstractValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {
        RuleFor(x => x.GoogleMapsPlusCode)
            .NotEmpty()
            .Must(BeAValidPlusCode)
            .WithMessage("Plus Code is invalid.");

        RuleFor(x => x.DeviceExtraInfo)
            .Custom((value, context) =>
            {
                if (string.IsNullOrEmpty(value)) return;
                if (value.Length > 255)
                {
                    context.AddFailure("DeviceExtraInfo max length is 255");
                }
            });

        RuleFor(x => x.MqttUsername)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(100)
            .WithMessage("MqttUsername length has to be between 3 and 100");
    }

    private static bool BeAValidPlusCode(string plusCode)
    {
        return OpenLocationCode.IsValid(plusCode);
    }
}
