using BerkutLampService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tuya.Net;
using Tuya.Net.Data;

namespace Berkut.LampService
{
    public class Lamp(ILogger<Lamp> logger, ITuyaClient tuyaClient, IOptionsSnapshot<LampOptions> options)
    {
        private readonly ILogger<Lamp> _logger = logger;
        private readonly ITuyaClient _tuyaClient = tuyaClient;
        private readonly LampOptions _options = options.Value;

        [Function("toggle")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var berkutLamp = await _tuyaClient.DeviceManager.GetDeviceAsync(_options.LampDeviceId) 
                ?? throw new InvalidOperationException("Cannot get lamp device");

            var lampStatus = berkutLamp?.StatusList?.FirstOrDefault(s => _options.LampStatusCode.Equals(s.Code, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException("Cannot get lamp status");

            if (lampStatus.Value is not bool)
                throw new ArgumentException($"Wrong lamp status: {lampStatus.Value}", nameof(lampStatus));

            await _tuyaClient.DeviceManager.SendCommandAsync(berkutLamp.Id, new Command()
            { 
                Code = _options.LampStatusCode, 
                Value = !(bool)lampStatus.Value 
            });

            return new OkResult();
        }
    }
}
