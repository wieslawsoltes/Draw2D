// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core.Containers;
using Draw2D.Core.Editor.Bounds;
using Draw2D.Core.Renderers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Editor.Tools
{
    public partial class PathTool : IToolContext
    {
        private PointShape _nextPoint;
        private IToolContext _context;

        private void SetContext(IToolContext context)
        {
            _context = context;
        }

        private void SetRenderer(ShapeRenderer renderer)
        {
            if (_context != null)
            {
                _context.Renderer = renderer;
            }
        }

        private void SetSelected(ISet<ShapeObject> selected)
        {
            if (_context != null)
            {
                _context.Selected = selected;
            }
        }

        private void SetNextPoint(PointShape point)
        {
            _nextPoint = point;
        }

        public ShapeRenderer Renderer
        {
            get => _context?.Renderer;
            set => SetRenderer(value);
        }

        public ISet<ShapeObject> Selected
        {
            get => _context?.Selected;
            set => SetSelected(value);
        }

        public IHitTest HitTest { get; set; }

        public IShapesContainer CurrentContainer
        {
            get => _figure;
            set => throw new InvalidCastException("Can't cast current container as a figure.");
        }

        public IShapesContainer WorkingContainer
        {
            get => _figure;
            set => throw new InvalidCastException("Can't cast current container as a figure.");
        }

        public DrawStyle CurrentStyle { get; set; }

        public ShapeObject PointShape { get; set; }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            return _nextPoint ?? _context?.GetNextPoint(x, y, connect, radius);
        }
    }
}
