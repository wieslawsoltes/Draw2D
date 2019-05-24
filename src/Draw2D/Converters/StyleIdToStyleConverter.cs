// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;

namespace Draw2D.Converters
{
    public class StyleIdToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string styleId && parameter is IStyleLibrary styleLibrary)
            {
                return styleLibrary.Get(styleId);
            }
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ShapeStyle style)
            {
                return style.Title;
            }
            return AvaloniaProperty.UnsetValue;
        }
    }
}
