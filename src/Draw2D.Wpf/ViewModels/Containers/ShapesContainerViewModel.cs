using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Editor;
using Draw2D.Editor.Bounds;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.Models.Renderers;
using Draw2D.Models.Style;

namespace Draw2D.ViewModels.Containers
{
    public class ShapesContainerViewModel : ViewModelBase, IToolContext
    {
        private ObservableCollection<ToolBase> _tools;
        private ToolBase _currentTool;
        private ShapeRenderer _renderer;
        private ISet<BaseShape> _selected;
        private IShapesContainer _currentContainer;
        private IShapesContainer _workingContainer;
        private DrawStyle _style;
        private BaseShape _pointShape;
        private HitTest _hitTest;

        public ObservableCollection<ToolBase> Tools
        {
            get { return _tools; }
            set
            {
                if (value != _tools)
                {
                    _tools = value;
                    Notify("Tools");
                }
            }
        }

        public ToolBase CurrentTool
        {
            get { return _currentTool; }
            set
            {
                if (value != _currentTool)
                {
                    if (_currentTool != null)
                    {
                        _currentTool.Clean(this);
                    }
                    _currentTool = value;
                    Notify("CurrentTool");
                }
            }
        }

        public ShapeRenderer Renderer
        {
            get { return _renderer; }
            set
            {
                if (value != _renderer)
                {
                    _renderer = value;
                    Notify("Renderer");
                }
            }
        }

        public ISet<BaseShape> Selected
        {
            get { return _selected; }
            set
            {
                if (value != _selected)
                {
                    _selected = value;
                    Notify("Selected");
                }
            }
        }

        public IShapesContainer CurrentContainer
        {
            get { return _currentContainer; }
            set
            {
                if (value != _currentContainer)
                {
                    _currentContainer = value;
                    Notify("Container");
                }
            }
        }

        public IShapesContainer WorkingContainer
        {
            get { return _workingContainer; }
            set
            {
                if (value != _workingContainer)
                {
                    _workingContainer = value;
                    Notify("WorkingContainer");
                }
            }
        }

        public DrawStyle Style
        {
            get { return _style; }
            set
            {
                if (value != _style)
                {
                    _style = value;
                    Notify("Style");
                }
            }
        }

        public BaseShape PointShape
        {
            get { return _pointShape; }
            set
            {
                if (value != _pointShape)
                {
                    _pointShape = value;
                    Notify("PointShape");
                }
            }
        }

        public HitTest HitTest
        {
            get { return _hitTest; }
            set
            {
                if (value != _hitTest)
                {
                    _hitTest = value;
                    Notify("HitTest");
                }
            }
        }
    }
}
