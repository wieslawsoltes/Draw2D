// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public abstract class PointIntersectionBase
    {
        public abstract string Title { get; }
        public IList<PointShape> Intersections { get; set; }

        protected PointIntersectionBase()
        {
            Intersections = new ObservableCollection<PointShape>();
        }

        public abstract void Find(IToolContext context, BaseShape shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.WorkingContainer.Shapes.Remove(point);
                context.Selection.Selected.Remove(point);
            }
            Intersections.Clear();
        }
    }
}
