using FluentValidation;

namespace WebUI.Validators;

public class PaceValidator : AbstractValidator<double>
{
    public PaceValidator()
    {
        RuleFor(v => v)
            .GreaterThanOrEqualTo(0).WithMessage("Pace must be non-negative.")
            .Must(v =>
            {
                var d = (decimal)v;
                var minutes = decimal.Truncate(d);
                var seconds = (int)((d - minutes) * 100m);
                return seconds is >= 0 and <= 59;
            }).WithMessage("Use mm.ss where ss is between 00 and 59.");
    }
    private IEnumerable<string> ValidateValue(double arg)
    {
        var result = Validate(arg);
        if (result.IsValid)
            return new string[0];
        return result.Errors.Select(e => e.ErrorMessage);
    }

    public Func<double, IEnumerable<string>> Validation => ValidateValue;
}
