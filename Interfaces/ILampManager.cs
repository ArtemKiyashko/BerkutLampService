using Tuya.Net.Data;

namespace BerkutLampService.Interfaces;

public interface ILampManager
{
    Task<Device> GetBerkutLampAsync();
    Task<bool> BerkutLampToggleAsync(Device device);
    Task<bool> BerkutLampSetStateAsync(Device device, bool state);
}
