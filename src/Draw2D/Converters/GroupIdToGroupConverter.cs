using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Converters
{
    public class GroupIdToGroupConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Count == 2 && values[0] is string groupId && values[1] is IGroupLibrary groupLibrary)
            {
                return groupLibrary.Get(groupId);
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
