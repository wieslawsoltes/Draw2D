// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers
{
    public interface IStyleLibrary : IDirty
    {
        IList<ShapeStyle> Styles { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        void UpdateCache();
        void Add(ShapeStyle value);
        void Remove(ShapeStyle value);
        ShapeStyle Get(string styleId);
    }
}
