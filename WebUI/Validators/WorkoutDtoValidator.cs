using FluentValidation;
using WebUI.Services.Dtos;

namespace WebUI.Validators
{
    public class WorkoutDtoValidator : AbstractValidator<WorkoutDto>
    {
        public WorkoutDtoValidator()
        {
            RuleFor(x => x.WeekNumber).GreaterThan(0).WithMessage("Week number must be greater than 0.");
            RuleFor(x => x.RunType).IsInEnum().WithMessage("Run type is not valid.");
            RuleFor(x => x.TotalDuration).GreaterThan(0).WithMessage("Total duration must be greater than 0.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description must not be empty.");
        }
    }
}
