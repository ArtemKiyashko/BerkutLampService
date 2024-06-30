using System.Text.Json;
using AutoMapper;
using BerkutLampService.Models;
using Tuya.Net.Data;
using DeviceStatus = Tuya.Net.Data.DeviceStatus;

namespace BerkutLampService.Mappers;

public class DeviceDataToDeviceMapper : ITypeConverter<DeviceData, Device>
{
    public Device Convert(DeviceData source, Device destination, ResolutionContext context)
    {
        destination = new Device
        {
            Id = source.DevId,
            StatusList = new List<DeviceStatus>(source.Status.Count)
        };

        foreach (var status in source.Status)
            destination.StatusList.Add(new DeviceStatus
            {
                Code = status.Code,
                Value = status.Value
            });

        return destination;
    }
}
