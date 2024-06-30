using AutoMapper;
using BerkutLampService.Models;
using Tuya.Net.Data;

namespace BerkutLampService.Mappers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<DeviceData, Device>().ConvertUsing<DeviceDataToDeviceMapper>();
    }
}
