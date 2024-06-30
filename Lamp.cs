using System.Text.Json;
using AutoMapper;
using BerkutLampService.Interfaces;
using BerkutLampService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Berkut.LampService;

public class Lamp(ILogger<Lamp> logger, ILampManager lampManager, IMapper mapper)
{
    private readonly ILogger<Lamp> _logger = logger;
    private readonly ILampManager _lampManager = lampManager;
    private readonly IMapper _mapper = mapper;

    [Function("toggle")]
    public async Task<IActionResult> Toggle([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var lampDevice = await _lampManager.GetBerkutLampAsync();
            await _lampManager.BerkutLampToggleAsync(lampDevice);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }
        return new OkResult();
    }

    [Function("setstate")]
    public async Task<IActionResult> SetState([HttpTrigger(AuthorizationLevel.Function, "get", Route = "setstate/{state:bool}")] HttpRequest req, bool state)
    {
        try
        {
            var lampDevice = await _lampManager.GetBerkutLampAsync();
            await _lampManager.BerkutLampSetStateAsync(lampDevice, state);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }
        return new OkResult();
    }

    [Function("getstate")]
    public async Task<IActionResult> GetState([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var lampDevice = await _lampManager.GetBerkutLampAsync();
            var lampState = _lampManager.BerkutLampGetState(lampDevice);
            return new OkObjectResult(new LampStateResponse() { State = lampState });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new BadRequestObjectResult(ex.Message);
        }
    }

    [Function("devicestatus")]
    [SignalROutput(HubName = "lampstatus")]
    public async Task<SignalRMessageAction?> ReportDeviceStatus([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var deviceData = await req.ReadFromJsonAsync<DeviceData>();

        if (deviceData is null || deviceData.Status is null)
            throw new ArgumentException("Cannot read device data");

        var tuyaDevice = _mapper.Map<Tuya.Net.Data.Device>(deviceData);
        var lampStatus = _lampManager.BerkutLampGetState(tuyaDevice);

        return new SignalRMessageAction("lampstatuschanged", [new LampStatusMessage(lampStatus)]);
    }

    [Function("negotiate")]
    public static IActionResult Negotiate([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
    [SignalRConnectionInfoInput(HubName = "lampstatus")] string connectionInfo) =>
        new OkObjectResult(connectionInfo);
}

