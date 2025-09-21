using GarminRunerz.Workout.Services.Models;
using System.Globalization;

namespace WebUI.Mappers
{
    public static class CustomWorkoutMapper
    {
        public static CustomWorkout FromCsvLine(string[] line)
        {
            if(line.Length != 9)
            {
                throw new ArgumentException($"Invalid CSV line. Expected 9 fields but got {line.Length}.");
            }
            return new CustomWorkout
            {
                WeekNumber = int.Parse(line[0]),
                RunType = Enum.Parse<RunType>(line[1], true),
                TotalDuration = int.Parse(line[2]),
                Repetitions = int.Parse(line[3]),
                RunDuration = double.Parse(line[4]),
                CoolDownDuration = double.Parse(line[5]),
                Pace = decimal.Parse(line[6], CultureInfo.InvariantCulture),
                Speed = decimal.Parse(line[7], CultureInfo.InvariantCulture),
                Description = line[8]
            };
        }
    }
}
