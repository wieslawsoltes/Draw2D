﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Intersections
{
    [DataContract(IsReference = true)]
    public abstract class PointIntersection : ViewModelBase, IPointIntersection
    {
        private IList<IPointShape> _intersections;

        public abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointShape> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        public abstract void Find(IToolContext context, IBaseShape shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(point);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(point);
            }
            Intersections.Clear();
        }
    }
}