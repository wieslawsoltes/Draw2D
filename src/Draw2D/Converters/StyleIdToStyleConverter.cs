using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Converters
{
    public class StyleIdToStyleConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Count == 2 && values[0] is string styleId && values[1] is IStyleLibrary styleLibrary)
            {
                return styleLibrary.Get(styleId);
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
