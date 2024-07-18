using System.Diagnostics;

namespace Fanex.DepositAPI.Infrastructure.Utils;

public static class Converter
{
    [DebuggerStepThrough]
    public static decimal ToDecimal(object value, decimal defaultValue = 0)
            => decimal.TryParse(ToString(value), out decimal _value) ? _value : defaultValue;

    [DebuggerStepThrough]
    public static bool ToBoolean(object value)
        => bool.TryParse(ToString(value), out bool _value) && _value;

    [DebuggerStepThrough]
    public static int ToInt(object value, int defaultValue = 0)
        => int.TryParse(ToString(value), out int _value) ? _value : defaultValue;

    [DebuggerStepThrough]
    public static string ToString(object value, string defaultValue = "")
        => value?.ToString() ?? defaultValue;

    [DebuggerStepThrough]
    public static DateTime ToDateTime(object value)
        => DateTime.TryParse(ToString(value), out DateTime _value) ? _value : default;
}