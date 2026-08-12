using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ProfiDesktop.Converters;

public class StrToListConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
        {
            return str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }
        return new List<string>();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
