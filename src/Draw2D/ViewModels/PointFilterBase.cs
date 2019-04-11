// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels
{
    public abstract class PointFilterBase
    {
        public abstract string Title { get; }
        public IList<BaseShape> Guides { get; set; }

        protected PointFilterBase()
        {
            Guides = new ObservableCollection<BaseShape>();
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.WorkingContainer.Shapes.Remove(guide);
                context.Selection.Selected.Remove(guide);
            }
            Guides.Clear();
        }
    }
}
