using BerkutLampService.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Options;
using Tuya.Net.Data;

namespace BerkutLampService.Managers;

public class LampManager : ILampManager
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly IValidator<Device> _lampDeviceValidator;
    private readonly LampOptions _options;

    public LampManager(IDeviceRepository deviceRepository, IOptionsSnapshot<LampOptions> options, IValidator<Device> lampDeviceValidator)
    {
        _deviceRepository = deviceRepository;
        _lampDeviceValidator = lampDeviceValidator;
        _options = options.Value;
    }

    public Task<bool> BerkutLampSetStateAsync(Device device, bool state) =>
        _deviceRepository.SendDeviceCommandAsync(device, new Command()
        {
            Code = _options.LampStatusCode,
            Value = state
        });

    public Task<bool> BerkutLampToggleAsync(Device device)
    {
        var lampStatus = device.StatusList.First(s => _options.LampStatusCode.Equals(s.Code, StringComparison.OrdinalIgnoreCase));
        return BerkutLampSetStateAsync(device, !(bool)lampStatus.Value);
    }

    public async Task<Device> GetBerkutLampAsync()
    {
        var lampDevice = await _deviceRepository.GetDeviceByIdAsync(_options.LampDeviceId);
        var validationResult = _lampDeviceValidator.Validate(lampDevice);
        if (!validationResult.IsValid)
            throw new InvalidOperationException(validationResult.ToString());
        return lampDevice;
    }
}
