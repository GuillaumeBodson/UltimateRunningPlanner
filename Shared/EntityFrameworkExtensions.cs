using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Shared;

public static class EntityFrameworkExtensions
{
    public static PropertyBuilder<T> UseJsonConversion<T>(this PropertyBuilder<T> propertyBuilder)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var converter = new ValueConverter<T, string>(
            value => JsonSerializer.Serialize(value, options),
            value => JsonSerializer.Deserialize<T>(value, options)!
        );

        propertyBuilder.HasConversion(converter);
        return propertyBuilder;
    }
}