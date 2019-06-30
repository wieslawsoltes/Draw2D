// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class StyleLibrary : Library<ShapeStyle>, IStyleLibrary
    {
        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new StyleLibrary()
            {
                Name = this.Name,
                CurrentItem = (ShapeStyle)this.CurrentItem?.Copy(shared),
                Items = new ObservableCollection<ShapeStyle>()
            };

            foreach (var item in this.Items)
            {
                if (item is ICopyable copyable)
                {
                    copy.Items.Add((ShapeStyle)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

    }
}
