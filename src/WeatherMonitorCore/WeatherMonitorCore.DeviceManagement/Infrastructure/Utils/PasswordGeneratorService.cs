using PasswordGenerator;

namespace WeatherMonitorCore.DeviceManagement.Infrastructure.Utils;

internal interface IPasswordGeneratorService
{
    string GeneratePassword();
}

internal class PasswordGeneratorService : IPasswordGeneratorService
{
    private readonly IPassword _passwordGenerator;
    private const int PasswordLength = 16;

    public PasswordGeneratorService()
    {
        _passwordGenerator = new Password()
            .IncludeLowercase()
            .IncludeUppercase()
            .IncludeNumeric()
            .IncludeSpecial()
            .LengthRequired(PasswordLength);
    }

    public string GeneratePassword() => _passwordGenerator.Next();
}
