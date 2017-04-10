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

        public IHitTest HitTest
        {
            get => _context?.HitTest;
            set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
        }

        public IShapesContainer CurrentContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {CurrentContainer} property value.");
        }

        public IShapesContainer WorkingContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {WorkingContainer} property value.");
        }

        public DrawStyle CurrentStyle
        {
            get => _context?.CurrentStyle;
            set => throw new InvalidOperationException($"Can not set {CurrentStyle} property value.");
        }

        public ShapeObject PointShape
        {
            get => _context?.PointShape;
            set => throw new InvalidOperationException($"Can not set {PointShape} property value.");
        }

        public Action Capture
        {
            get => _context?.Capture;
            set => throw new InvalidOperationException($"Can not set {Capture} property value.");
        }

        public Action Release
        {
            get => _context?.Release;
            set => throw new InvalidOperationException($"Can not set {Release} property value.");
        }

        public Action Invalidate
        {
            get => _context?.Invalidate;
            set => throw new InvalidOperationException($"Can not set {Invalidate} property value.");
        }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            return _nextPoint ?? _context?.GetNextPoint(x, y, connect, radius);
        }
    }
}
