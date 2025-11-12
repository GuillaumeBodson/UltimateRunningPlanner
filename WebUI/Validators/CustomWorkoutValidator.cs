using FluentValidation;
using GarminRunerz.Workout.Services.Models;

namespace WebUI.Validators
{
    public class CustomWorkoutValidator : AbstractValidator<CustomWorkout>
    {
        public CustomWorkoutValidator()
        {
            //RuleFor(x => x.WeekNumber).GreaterThan(0).WithMessage("Week number must be greater than 0.");
            RuleFor(x => x.RunType).IsInEnum().WithMessage("Run type is not valid.");
            RuleFor(x => x.TotalDuration).GreaterThan(0).WithMessage("Total duration must be greater than 0.");
            //RuleFor(x => x.Repetitions).GreaterThanOrEqualTo(0).WithMessage("Repetitions must be greater than 0.");
            //RuleFor(x => x.RunDuration).GreaterThanOrEqualTo(0).WithMessage("Run duration must be greater than or equal to 0.");
            //RuleFor(x => x.CoolDownDuration).GreaterThanOrEqualTo(0).WithMessage("Cool down duration must be greater than or equal to 0.");
            //RuleFor(x => x.Pace).GreaterThanOrEqualTo(0).WithMessage("Pace must be greater than or equal to 0.");
            //RuleFor(x => x.Speed).GreaterThanOrEqualTo(0).WithMessage("Speed must be greater than or equal to 0.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description must not be empty.");
        }
    }
}
