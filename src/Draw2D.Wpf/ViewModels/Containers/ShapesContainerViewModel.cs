using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Editor;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Renderers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;

namespace Draw2D.ViewModels.Containers
{
    public class ShapesContainerViewModel : ViewModelBase, IToolContext
    {
        private ObservableCollection<ToolBase> _tools;
        private ToolBase _currentTool;
        private ShapeRenderer _renderer;
        private ISet<ShapeObject> _selected;
        private IShapesContainer _currentContainer;
        private IShapesContainer _workingContainer;
        private DrawStyle _style;
        private ShapeObject _pointShape;
        private HitTest _hitTest;

        public ObservableCollection<ToolBase> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        public ToolBase CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool?.Clean(this);
                Update(ref _currentTool, value);
            }
        }

        public ShapeRenderer Renderer
        {
            get => _renderer;
            set => Update(ref _renderer, value);
        }

        public ISet<ShapeObject> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public IShapesContainer CurrentContainer
        {
            get => _currentContainer;
            set => Update(ref _currentContainer, value);
        }

        public IShapesContainer WorkingContainer
        {
            get => _workingContainer;
            set => Update(ref _workingContainer, value);
        }

        public DrawStyle CurrentStyle
        {
            get => _style;
            set => Update(ref _style, value);
        }

        public ShapeObject PointShape
        {
            get => _pointShape;
            set => Update(ref _pointShape, value);
        }

        public HitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        public PointShape GetNextPoint(double x, double y) => new PointShape(x, y, PointShape);

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }
    }
}
