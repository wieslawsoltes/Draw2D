// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Containers
{
    public interface IGroupLibrary : IDirty
    {
        IList<GroupShape> Groups { get; set; }
        GroupShape CurrentGroup { get; set; }
        void UpdateCache();
        void Add(GroupShape value);
        void Remove(GroupShape value);
        GroupShape Get(string groupId);
    }
}
