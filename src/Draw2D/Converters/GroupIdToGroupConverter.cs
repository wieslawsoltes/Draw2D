// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;

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
