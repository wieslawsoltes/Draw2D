// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Filters
{
    [DataContract(IsReference = true)]
    public abstract class PointFilter : ViewModelBase, IPointFilter
    {
        private IList<IBaseShape> _guides;

        public new abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IBaseShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(guide);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(guide);
            }
            Guides.Clear();
        }
    }
}
