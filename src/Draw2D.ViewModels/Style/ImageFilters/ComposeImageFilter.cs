// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ImageFilters
{
    [DataContract(IsReference = true)]
    public class ComposeImageFilter : ViewModelBase, IImageFilter
    {
        // TODO:

        public ComposeImageFilter()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ComposeImageFilter()
            {
                Title = this.Title
            };
        }
    }
}
