using BerkutLampService.Interfaces;
using BerkutLampService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Berkut.LampService;

public class Lamp(ILogger<Lamp> logger, ILampManager lampManager)
{
    private readonly ILogger<Lamp> _logger = logger;
    private readonly ILampManager _lampManager = lampManager;

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
}

