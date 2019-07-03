// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public abstract class BaseTool : ViewModelBase
    {
        private IList<IPointIntersection> _intersections;
        private IPointIntersection _currentIntersection;
        private IList<IPointFilter> _filters;
        private IPointFilter _currentFilter;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointIntersection CurrentIntersection
        {
            get => _currentIntersection;
            set => Update(ref _currentIntersection, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointFilter CurrentFilter
        {
            get => _currentFilter;
            set => Update(ref _currentFilter, value);
        }

        internal void FiltersProcess(IToolContext context, ref double x, ref double y)
        {
            if (_filters != null)
            {
                foreach (var filter in _filters)
                {
                    if (filter.Process(context, ref x, ref y))
                    {
                        return;
                    }
                }
            }
        }

        internal void FiltersClear(IToolContext context)
        {
            if (_filters != null)
            {
                foreach (var filter in _filters)
                {
                    filter.Clear(context);
                }
            }
        }

        internal void IntersectionsFind(IToolContext context, IBaseShape shape)
        {
            if (_intersections != null)
            {
                foreach (var intersection in _intersections)
                {
                    intersection.Find(context, shape);
                }
            }
        }

        internal void IntersectionsClear(IToolContext context)
        {
            if (_intersections != null)
            {
                foreach (var intersection in _intersections)
                {
                    intersection.Clear(context);
                }
            }
        }

        internal bool HaveIntersections()
        {
            if (_intersections != null)
            {
                foreach (var intersection in _intersections)
                {
                    if (intersection.Intersections.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
