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
            .Must(p => p.HasValue).WithMessage("Berkut lamp online status is unknown")
            .Must(p => p.Value).WithMessage("Berkut lamp is offline");

        RuleFor(x => x.StatusList).NotNull().WithMessage("Cannot get status list");

        RuleFor(x => x.StatusList.FirstOrDefault(s => _options.LampStatusCode.Equals(s.Code, StringComparison.OrdinalIgnoreCase)))
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Cannot get lamp status")
            .Must(p => p.Value is bool).WithMessage("Wrong lamp status");
    }

    protected override bool PreValidate(ValidationContext<Device> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure("", "Cannot get lamp device"));
            return false;
        }
        return true;
    }
}
