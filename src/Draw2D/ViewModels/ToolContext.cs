// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Draw2D.Input;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;

namespace Draw2D.ViewModels
{
    public abstract class ToolContext : ViewModelBase, IToolContext
    {
        private IInputService _inputService;
        private IShapeRenderer _renderer;
        private IHitTest _hitTest;
        private CanvasContainer _currentContainer;
        private CanvasContainer _workingContainer;
        private ShapeStyle _currentStyle;
        private BaseShape _pointShape;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private EditMode _mode;
        private ICanvasPresenter _presenter;
        private ISelection _selection;

        public IInputService ZoomControl
        {
            get => _inputService;
            set => Update(ref _inputService, value);
        }

        public IShapeRenderer Renderer
        {
            get => _renderer;
            set => Update(ref _renderer, value);
        }

        public IHitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        public CanvasContainer CurrentContainer
        {
            get => _currentContainer;
            set => Update(ref _currentContainer, value);
        }

        public CanvasContainer WorkingContainer
        {
            get => _workingContainer;
            set => Update(ref _workingContainer, value);
        }

        public ShapeStyle CurrentStyle
        {
            get => _currentStyle;
            set => Update(ref _currentStyle, value);
        }

        public BaseShape PointShape
        {
            get => _pointShape;
            set => Update(ref _pointShape, value);
        }

        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool?.Clean(this);
                Update(ref _currentTool, value);
            }
        }

        public EditMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public ICanvasPresenter Presenter
        {
            get => _presenter;
            set => Update(ref _presenter, value);
        }

        public ISelection Selection
        {
            get => _selection;
            set => Update(ref _selection, value);
        }

        public virtual PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            if (connect == true)
            {
                var point = HitTest.TryToGetPoint(CurrentContainer.Shapes, new Point2(x, y), radius, null);
                if (point != null)
                {
                    return point;
                }
            }
            return new PointShape(x, y, PointShape);
        }

        public void SetTool(string title)
        {
            if (CurrentTool is PathTool pathTool && pathTool.Settings.CurrentTool.Title != title)
            {
                pathTool.CleanCurrentTool(this);
                var tool = pathTool.Settings.Tools.Where(t => t.Title == title).FirstOrDefault();
                if (tool != null)
                {
                    pathTool.Settings.CurrentTool = tool;
                }
                else
                {
                    CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
                }
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
            }
        }

        public void LeftDown(double x, double y, Modifier modifier)
        {
            _currentTool.LeftDown(this, x, y, modifier);
        }

        public void LeftUp(double x, double y, Modifier modifier)
        {
            if (_mode == EditMode.Mouse)
            {
                _currentTool.LeftUp(this, x, y, modifier);
            }
            else if (_mode == EditMode.Touch)
            {
                _currentTool.LeftDown(this, x, y, modifier);
            }
        }

        public void RightDown(double x, double y, Modifier modifier)
        {
            _currentTool.RightDown(this, x, y, modifier);
        }

        public void RightUp(double x, double y, Modifier modifier)
        {
            _currentTool.RightUp(this, x, y, modifier);
        }

        public void Move(double x, double y, Modifier modifier)
        {
            _currentTool.Move(this, x, y, modifier);
        }

        public double GetWidth()
        {
            return _currentContainer?.Width ?? 0.0;
        }

        public double GetHeight()
        {
            return _currentContainer?.Height ?? 0.0;
        }
    }
}
