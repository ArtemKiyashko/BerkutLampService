using BerkutLampService.Interfaces;
using Tuya.Net;
using Tuya.Net.Data;

namespace BerkutLampService.Repositories;

public class DeviceRepository(ITuyaClient tuyaClient) : IDeviceRepository
{
    private readonly ITuyaClient _tuyaClient = tuyaClient;

    public Task<Device?> GetDeviceByIdAsync(string id) => _tuyaClient.DeviceManager.GetDeviceAsync(id);

    public Task<bool> SendDeviceCommandAsync(Device device, Command command) => _tuyaClient.DeviceManager.SendCommandAsync(device, command);
}
