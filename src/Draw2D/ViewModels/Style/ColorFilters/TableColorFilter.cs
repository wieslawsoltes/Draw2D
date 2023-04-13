﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.ColorFilters;

[DataContract(IsReference = true)]
public class TableColorFilter : ViewModelBase, IColorFilter
{
    // TODO:

    public TableColorFilter()
    {
    }

    public object Copy(Dictionary<object, object> shared)
    {
        return new TableColorFilter()
        {
            Title = this.Title
        };
    }
}