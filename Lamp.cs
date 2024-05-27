using BerkutLampService;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tuya.Net;
using Tuya.Net.Data;

namespace Berkut.LampService
{
    public class Lamp(ILogger<Lamp> logger, ITuyaClient tuyaClient, IOptionsSnapshot<LampOptions> options, IValidator<Device> validator)
    {
        private readonly ILogger<Lamp> _logger = logger;
        private readonly ITuyaClient _tuyaClient = tuyaClient;
        private readonly LampOptions _options = options.Value;
        private readonly IValidator<Device> _validator = validator;

        [Function("toggle")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            var berkutLamp = await _tuyaClient.DeviceManager.GetDeviceAsync(_options.LampDeviceId);

            var validationResult = _validator.Validate(berkutLamp);

            if (!validationResult.IsValid)
            {
                _logger.LogError(validationResult.ToString());
                return new BadRequestObjectResult(validationResult);
            }

            var lampStatus = berkutLamp.StatusList.First(s => _options.LampStatusCode.Equals(s.Code, StringComparison.OrdinalIgnoreCase));

            await _tuyaClient.DeviceManager.SendCommandAsync(berkutLamp.Id, new Command()
            {
                Code = _options.LampStatusCode,
                Value = !(bool)lampStatus.Value
            });

            return new OkResult();
        }
    }
}
