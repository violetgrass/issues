using System;
using System.Collections;
using System.Linq;

namespace Violet.WorkItems;

public static class CommonDataTypesExtensions
{
    public static void Value<T>(this Property property, T value)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        var type = typeof(T);
        if (!ValueTypesManager.IsTypeMatch(property.DataType, type))
        {
            throw new InvalidOperationException($"Cannot serialize {type.Name} to property {property.Name} of data type {property.DataType}");
        }

        var tc = ValueTypesManager.GetConverter(type);

        if (tc is null)
        {
            throw new InvalidOperationException($"No Type Coverter found for type '{type}'.");
        }

        var result = tc.ConvertToInvariantString(value);

        property.Value = result ?? string.Empty;
    }

    public static void Value<T>(this Property property, out T? value)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        var type = typeof(T);
        if (!ValueTypesManager.IsTypeMatch(property.DataType, type))
        {
            throw new InvalidOperationException($"Cannot deserialize {type.Name} from property {property.Name} of data type {property.DataType}");
        }

        var tc = ValueTypesManager.GetConverter(type);

        if (tc is null)
        {
            throw new InvalidOperationException($"No Type Coverter found for type '{type}'.");
        }

        if (tc.IsValid(property.Value))
        {
            var result = tc.ConvertFromInvariantString(property.Value);

            value = (T?)result;
        }
        else
        {
            throw new ArgumentException($"Property value '{property.Value}' cannot be converted to {type.Name}");
        }
    }

    public static void NullableValue<T>(this Property property, T? value) where T : struct
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (!value.HasValue)
        {
            property.Value = Property.NullValue;
        }
        else
        {
            Value<T>(property, value.Value);
        }
    }

    public static void NullableValue<T>(this Property property, T? value) where T : class
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (value == null)
        {
            property.Value = Property.NullValue;
        }
        else
        {
            Value<T>(property, value);
        }
    }

    public static void NullableValue<T>(this Property property, out T? value) where T : struct
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (property.Value == null || property.Value == string.Empty)
        {
            value = default;
        }
        else
        {
            Value<T>(property, out var notNullableValue2);
            value = notNullableValue2;
        }
    }

    public static void NullableValue<T>(this Property property, out T? value) where T : class
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (property.Value == null || property.Value == string.Empty)
        {
            value = default;
        }
        else
        {
            Value<T>(property, out var notNullableValue2);
            value = notNullableValue2;
        }
    }


    public static void Values<T>(this Property property, params T[] values)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        var type = typeof(T);
        if (!ValueTypesManager.IsTypeMatch(property.DataType, type))
        {
            throw new InvalidOperationException($"Cannot serialize {type.Name} to property {property.Name} of data type {property.DataType}");
        }

        var tc = ValueTypesManager.GetConverter(type);

        if (tc is null)
        {
            throw new InvalidOperationException($"No Type Coverter found for type '{type}'.");
        }

        var valuesAsString = values.Select(v => tc.ConvertToInvariantString(v)).ToArray();

        var result = string.Join(",", valuesAsString);

        property.Value = result;
    }

    public static void Values<T>(this Property property, out T[] values)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        var type = typeof(T);
        if (!ValueTypesManager.IsTypeMatch(property.DataType, type))
        {
            throw new InvalidOperationException($"Cannot deserialize {type.Name} from property {property.Name} of data type {property.DataType}");
        }

        var tc = ValueTypesManager.GetConverter(type);

        if (tc is null)
        {
            throw new InvalidOperationException($"No Type Coverter found for type '{type}'.");
        }

        var stringValues = property.Value.Split(',');

        if (stringValues.All(v => tc.IsValid(v)))
        {
            var result = stringValues.Select(v => tc.ConvertFromInvariantString(v)).Cast<T>().ToArray();

            values = result;
        }
        else
        {
            throw new ArgumentException($"Property value '{property.Value}' cannot be converted to array of {type.Name}");
        }
    }
}
