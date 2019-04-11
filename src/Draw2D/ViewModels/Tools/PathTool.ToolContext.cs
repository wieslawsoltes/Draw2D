// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using PanAndZoom;

namespace Draw2D.ViewModels.Tools
{
    public partial class PathTool : IToolContext
    {
        private IToolContext _context;
        private PointShape _nextPoint;

        public IShapeRenderer Renderer
        {
            get => _context.Renderer;
            set => SetRenderer(value);
        }

        public IHitTest HitTest
        {
            get => _context.HitTest;
            set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
        }

        public CanvasContainer CurrentContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {CurrentContainer} property value.");
        }

        public CanvasContainer WorkingContainer
        {
            get => _figure;
            set => throw new InvalidOperationException($"Can not set {WorkingContainer} property value.");
        }

        public ShapeStyle CurrentStyle
        {
            get => _context.CurrentStyle;
            set => throw new InvalidOperationException($"Can not set {CurrentStyle} property value.");
        }

        public BaseShape PointShape
        {
            get => _context.PointShape;
            set => throw new InvalidOperationException($"Can not set {PointShape} property value.");
        }

        public Action Capture
        {
            get => _context.Capture;
            set => throw new InvalidOperationException($"Can not set {Capture} property value.");
        }

        public Action Release
        {
            get => _context.Release;
            set => throw new InvalidOperationException($"Can not set {Release} property value.");
        }

        public Action Invalidate
        {
            get => _context.Invalidate;
            set => throw new InvalidOperationException($"Can not set {Invalidate} property value.");
        }

        public IList<ToolBase> Tools
        {
            get => _context.Tools;
            set => throw new InvalidOperationException($"Can not set {Tools} property value.");
        }

        public ToolBase CurrentTool
        {
            get => _context.CurrentTool;
            set => throw new InvalidOperationException($"Can not set {CurrentTool} property value.");
        }

        public EditMode Mode
        {
            get => _context.Mode;
            set => throw new InvalidOperationException($"Can not set {Mode} property value.");
        }

        public ICanvasPresenter Presenter
        {
            get => _context.Presenter;
            set => throw new InvalidOperationException($"Can not set {Presenter} property value.");
        }

        public ISelection Selection
        {
            get => _context.Selection;
            set => throw new InvalidOperationException($"Can not set {Selection} property value.");
        }

        public IPanAndZoom Zoom
        {
            get => _context.Zoom;
            set => throw new InvalidOperationException($"Can not set {Zoom} property value.");
        }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            return _nextPoint ?? _context.GetNextPoint(x, y, connect, radius);
        }

        public void SetTool(string name)
        {
            _context.SetTool(name);
        }

        private void SetContext(IToolContext context)
        {
            _context = context;
        }

        private void SetRenderer(IShapeRenderer renderer)
        {
            if (_context != null)
            {
                _context.Renderer = renderer;
            }
        }

        private void SetNextPoint(PointShape point)
        {
            _nextPoint = point;
        }
    }
}
