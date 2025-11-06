using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;
using WebUI.Converters;

namespace WebUI.Models
{
    // Represents pace (per km) as total seconds; formats/parses as mm:ss.
    [JsonConverter(typeof(PaceJsonConverter))]
    public readonly struct Pace :
        IEquatable<Pace>,
        IComparable<Pace>,
        IParsable<Pace>,
        ISpanFormattable,
        IAdditionOperators<Pace, Pace, Pace>,
        ISubtractionOperators<Pace, Pace, Pace>,
        IMultiplyOperators<Pace, double, Pace>,
        IDivisionOperators<Pace, double, Pace>
    {
        private readonly int _totalSeconds; // per kilometer

        public Pace(int totalSeconds)
        {
            if (totalSeconds < 0) throw new ArgumentOutOfRangeException(nameof(totalSeconds), "Pace must be non-negative.");
            _totalSeconds = totalSeconds;
        }

        public int TotalSeconds => _totalSeconds;
        public double ToMinutesDouble() => _totalSeconds / 60d;
        public double ToMinutesDotSeconds()
        {
            var minutes = _totalSeconds / 60;
            var seconds = _totalSeconds % 60;
            return minutes + seconds / 100d;
        }

        public static Pace Zero => new(0);
        public static Pace One => FromMinutesSeconds(1, 0);

        public static Pace FromMinutesSeconds(int minutes, int seconds)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(minutes);
            if (seconds is < 0 or > 59) throw new ArgumentOutOfRangeException(nameof(seconds), "Seconds must be in [0, 59].");
            return new Pace(checked(minutes * 60 + seconds));
        }
        public static Pace FromMinutesSeconds(double time)
        {
            int minutes = (int)Math.Floor(time);
            int seconds = (int)Math.Round((time - minutes) * 100);

            return FromMinutesSeconds(minutes, seconds);
        }

        public static Pace FromMinutesDecimal(decimal minutes)
        {
            if (minutes < 0m)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Must be a finite, non-negative value.");
            var secs = (int)Math.Round(minutes * 60m, MidpointRounding.AwayFromZero);
            return new Pace(secs);
        }

        public static Pace FromMinutesDouble(double minutes)
        {
            if (double.IsNaN(minutes) || double.IsInfinity(minutes) || minutes < 0d)
                throw new ArgumentOutOfRangeException(nameof(minutes), "Must be a finite, non-negative value.");
            var secs = (int)Math.Round(minutes * 60d, MidpointRounding.AwayFromZero);
            return new Pace(secs);
        }

        public decimal ToMeterPerSeconds() => _totalSeconds == 0 ? 0m : 1000m / _totalSeconds;
        public double ToKmPerHour() => _totalSeconds == 0 ? 0d : 3600d / _totalSeconds;

        // Arithmetic (saturate subtraction at 0)
        public static Pace operator +(Pace left, Pace right) => new(checked(left._totalSeconds + right._totalSeconds));
        public static Pace operator -(Pace left, Pace right) => new(Math.Max(0, left._totalSeconds - right._totalSeconds));
        public static Pace operator *(Pace left, double factor)
        {
            if (double.IsNaN(factor) || double.IsInfinity(factor)) throw new OverflowException("Invalid factor.");
            var secs = (int)Math.Round(left._totalSeconds * factor, MidpointRounding.AwayFromZero);
            if (secs < 0) throw new OverflowException("Result is negative.");
            return new Pace(secs);
        }
        public static Pace operator /(Pace left, double divisor)
        {
            if (divisor == 0d) throw new DivideByZeroException();
            if (double.IsNaN(divisor) || double.IsInfinity(divisor)) throw new OverflowException("Invalid divisor.");
            var secs = (int)Math.Round(left._totalSeconds / divisor, MidpointRounding.AwayFromZero);
            if (secs < 0) throw new OverflowException("Result is negative.");
            return new Pace(secs);
        }

        // Comparison / equality
        public int CompareTo(Pace other) => _totalSeconds.CompareTo(other._totalSeconds);
        public bool Equals(Pace other) => _totalSeconds == other._totalSeconds;
        public override bool Equals(object? obj) => obj is Pace p && Equals(p);
        public override int GetHashCode() => _totalSeconds;

        // Formatting: default "m:ss"; format "m" -> minutes as double; "s" -> total seconds
        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            format ??= "G";
            var minutes = _totalSeconds / 60;
            var seconds = _totalSeconds % 60;

            return format switch
            {
                "G" or "g" or "mm:ss" => $"{minutes}:{seconds:00}",
                "m" => ToMinutesDouble().ToString(formatProvider),
                "s" => _totalSeconds.ToString(formatProvider),
                _ => $"{minutes}:{seconds:00}"
            };
        }

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            // Only "G"/default supported in span formatting for simplicity
            var minutes = _totalSeconds / 60;
            var seconds = _totalSeconds % 60;
            var s = $"{minutes}:{seconds:00}";
            if (s.AsSpan().TryCopyTo(destination))
            {
                charsWritten = s.Length;
                return true;
            }
            charsWritten = 0;
            return false;
        }

        // Parsing: strictly "mm:ss" or "m:ss".
        // Additionally supports "m.ss" or "m,ss" where the fractional part is interpreted as seconds and must be [0..59].
        public static Pace Parse(string s, IFormatProvider? provider)
            => TryParse(s, provider, out var result) ? result : throw new FormatException("Invalid pace. Use mm:ss (e.g., 4:59).");

        public static Pace Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
            => TryParse(s.ToString(), provider, out var result) ? result : throw new FormatException("Invalid pace. Use mm:ss (e.g., 4:59).");

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Pace result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();

            // Colon form mm:ss
            var colonIdx = s.IndexOf(':');
            if (colonIdx > 0)
            {
                if (int.TryParse(s.AsSpan(0, colonIdx), NumberStyles.Integer, provider, out var m)
                    && int.TryParse(s.AsSpan(colonIdx + 1), NumberStyles.Integer, provider, out var sec)
                    && m >= 0 && sec is >= 0 and <= 59)
                {
                    result = FromMinutesSeconds(m, sec);
                    return true;
                }
                return false;
            }

            // Decimal separator interpreted as seconds (must be [0..59])
            var nfi = NumberFormatInfo.GetInstance(provider);
            var sep = nfi.NumberDecimalSeparator;
            var sepIdx = s.IndexOf(sep, StringComparison.Ordinal);
            if (sepIdx > 0)
            {
                if (int.TryParse(s.AsSpan(0, sepIdx), NumberStyles.Integer, provider, out var m)
                    && int.TryParse(s.AsSpan(sepIdx + sep.Length), NumberStyles.Integer, provider, out var sec)
                    && m >= 0 && sec is >= 0 and <= 59)
                {
                    // Treat "4.5" as "4:05" if one digit is provided; "4.50" as "4:50"
                    if (s.Length - (sepIdx + sep.Length) == 1) sec *= 10;
                    result = FromMinutesSeconds(m, sec);
                    return true;
                }
                return false;
            }

            // Plain integer minutes
            if (int.TryParse(s, NumberStyles.Integer, provider, out var minutes) && minutes >= 0)
            {
                result = FromMinutesSeconds(minutes, 0);
                return true;
            }

            return false;
        }

        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Pace result)
            => TryParse(s.ToString(), provider, out result);
    }
}