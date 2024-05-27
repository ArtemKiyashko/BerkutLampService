using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using Tuya.Net.Data;

namespace BerkutLampService.Validators;

public class LampDeviceValidator : AbstractValidator<Device>
{
    private readonly LampOptions _options;

    public LampDeviceValidator(IOptionsSnapshot<LampOptions> options)
    {
        _options = options.Value;

        RuleFor(x => x.IsOnline)
            .Cascade(CascadeMode.Stop)
            .Must(p => p.HasValue).WithMessage("Berkut lamp online status is unknown").WithErrorCode("LampNotFound")
            .Must(p => p.Value).WithMessage("Berkut lamp is offline").WithErrorCode("LampOffline");

        RuleFor(x => x.StatusList).NotNull().WithMessage("Cannot get status list").WithErrorCode("StatusListNotFound");

        RuleFor(x => x.StatusList.FirstOrDefault(s => _options.LampStatusCode.Equals(s.Code, StringComparison.OrdinalIgnoreCase)))
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Cannot get lamp status").WithErrorCode("LampStatusNotFound")
            .Must(p => p.Value is bool).WithMessage("Wrong lamp status").WithErrorCode("WrongLampStatus");
    }

    protected override bool PreValidate(ValidationContext<Device> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("", "Cannot get lamp device") { ErrorCode = "LampDeviceNotFound" });
            return false;
        }
        return true;
    }
}
