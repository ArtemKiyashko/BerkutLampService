using Tuya.Net.Data;

namespace BerkutLampService.Interfaces;

public interface IDeviceRepository
{
    Task<Device?> GetDeviceByIdAsync(string id);
    Task<bool> SendDeviceCommandAsync(Device device, Command command);

}
