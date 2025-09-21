using FluentValidation;
using GarminRunerz.Workout.Services.Models;

namespace WebUI.Validators
{
    public class CustomWokoutValidator : AbstractValidator<CustomWorkout>
    {
        public CustomWokoutValidator()
        {
            
        }
    }
}
