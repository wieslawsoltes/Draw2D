// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Core2D.Containers;
using Core2D.Editor;
using Core2D.Editor.Bounds;
using Core2D.Editor.Bounds.Shapes;
using Core2D.Editor.Filters;
using Core2D.Editor.Intersections.Line;
using Core2D.Editor.Selection;
using Core2D.Editor.Tools;
using Core2D.Editor.Tools.Helpers;
using Core2D.Json;
using Core2D.Presenters;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;

namespace Core2D.ViewModels
{
    public class MainViewModel : ObservableObject, IToolContext
    {
        private ObservableCollection<ToolBase> _tools;
        private ToolBase _currentTool;
        private EditMode _mode;
        private ShapePresenter _presenter;
        private ShapeRenderer _renderer;
        private IHitTest _hitTest;
        private CanvasContainer _currentContainer;
        private CanvasContainer _workingContainer;
        private ShapeStyle _currentStyle;
        private BaseShape _pointShape;

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

        public EditMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public ShapePresenter Presenter
        {
            get => _presenter;
            set => Update(ref _presenter, value);
        }

        public ShapeRenderer Renderer
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

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }

        public Action Reset { get; set; }

        public Action AutoFit { get; set; }

        public Action StretchNone { get; set; }

        public Action StretchFill { get; set; }

        public Action StretchUniform { get; set; }

        public Action StretchUniformToFill { get; set; }

        public static MainViewModel Load(string path)
        {
            var json = File.ReadAllText(path);
            var vm = NewtonsoftJsonSerializer.FromJson<MainViewModel>(json);
            return vm;
        }

        public static void Save(string path, MainViewModel vm)
        {
            var renderer = vm.Renderer;
            var currentContainer = vm.CurrentContainer;
            var workingContainer = vm.WorkingContainer;
            var capture = vm.Capture;
            var release = vm.Release;
            var invalidate = vm.Invalidate;
            var reset = vm.Reset;
            var autoFit = vm.AutoFit;
            var stretchNone = vm.StretchNone;
            var stretchFill = vm.StretchFill;
            var stretchUniform = vm.StretchUniform;
            var stretchUniformToFill = vm.StretchUniformToFill;

            vm.Renderer = null;
            vm.CurrentContainer = null;
            vm.WorkingContainer = null;
            vm.Capture = null;
            vm.Release = null;
            vm.Invalidate = null;
            vm.Reset = null;
            vm.AutoFit = null;
            vm.StretchNone = null;
            vm.StretchFill = null;
            vm.StretchUniform = null;
            vm.StretchUniformToFill = null;

            var json = NewtonsoftJsonSerializer.ToJson(vm);
            File.WriteAllText(path, json);

            vm.Renderer = renderer;
            vm.CurrentContainer = currentContainer;
            vm.WorkingContainer = workingContainer;
            vm.Capture = capture;
            vm.Release = release;
            vm.Invalidate = invalidate;
            vm.Reset = reset;
            vm.AutoFit = autoFit;
            vm.StretchNone = stretchNone;
            vm.StretchFill = stretchFill;
            vm.StretchUniform = stretchUniform;
            vm.StretchUniformToFill = stretchUniformToFill;
        }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
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

        public void New()
        {
            CurrentTool.Clean(this);
            Renderer.Selected.Clear();
            var container = new CanvasContainer()
            {
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 128, 128, 128),
                InputBackground = new ArgbColor(255, 211, 211, 211)
            };
            var workingContainer = new CanvasContainer();
            CurrentContainer = container;
            WorkingContainer = new CanvasContainer();
            Invalidate?.Invoke();
        }

        public void OpenAsJson(string path)
        {
            var json = File.ReadAllText(path);
            var container = NewtonsoftJsonSerializer.FromJson<CanvasContainer>(json);
            var workingContainer = new CanvasContainer();
            CurrentTool.Clean(this);
            Renderer.Selected.Clear();
            CurrentContainer = container;
            WorkingContainer = workingContainer;
        }

        public void SaveAsJson(string path)
        {
            var json = NewtonsoftJsonSerializer.ToJson(CurrentContainer);
            File.WriteAllText(path, json);
        }

        public async void Open()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                OpenAsJson(path);
                Invalidate?.Invoke();
            }
        }

        public async void SaveAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = "container";
            dlg.DefaultExtension = "project";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                SaveAsJson(path);
            }
        }

        public void Exit()
        {
            Application.Current.Windows.FirstOrDefault()?.Close();
        }

        public void Cut()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Cut(this);
            }
        }

        public void Copy()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Copy(this);
            }
        }

        public void Paste()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Paste(this);
            }
        }

        public void Delete()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Delete(this);
            }
        }

        public void Group()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Group(this);
            }
        }

        public void SelectAll()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.SelectAll(this);
            }
        }

        public void SetTool(string name)
        {
            if (CurrentTool is PathTool pathTool)
            {
                pathTool.CleanCurrentTool(this);
                var tool = pathTool.Settings.Tools.Where(t => t.Title == name).FirstOrDefault();
                if (tool != null)
                {
                    pathTool.Settings.CurrentTool = tool;
                }
                else
                {
                    CurrentTool = Tools.Where(t => t.Title == name).FirstOrDefault();
                }
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == name).FirstOrDefault();
            }
        }
    }

    public class MainViewModelFactory
    {
        public MainViewModel CreateMainViewModel()
        {
            var hitTest = new HitTest();

            hitTest.Register(new PointHitTest());
            hitTest.Register(new LineHitTest());
            hitTest.Register(new CubicBezierHitTest());
            hitTest.Register(new QuadraticBezierHitTest());
            hitTest.Register(new GroupHitTest());
            hitTest.Register(new PathHitTest());
            hitTest.Register(new RectangleHitTest());
            hitTest.Register(new EllipseHitTest());
            hitTest.Register(new TextHitTest());

            var gridSnapPointFilter = new GridSnapPointFilter()
            {
                Settings = new GridSnapSettings()
                {
                    IsEnabled = true,
                    EnableGuides = false,
                    Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                    GridSizeX = 15.0,
                    GridSizeY = 15.0,
                    GuideStyle = new ShapeStyle(new ArgbColor(128, 0, 255, 255), new ArgbColor(128, 0, 255, 255), 2.0, true, true)
                }
            };

            var lineSnapPointFilter = new LineSnapPointFilter()
            {
                Settings = new LineSnapSettings()
                {
                    IsEnabled = true,
                    EnableGuides = false,
                    Target = LineSnapTarget.Guides | LineSnapTarget.Shapes,
                    Mode = LineSnapMode.Point
                    | LineSnapMode.Middle
                    | LineSnapMode.Nearest
                    | LineSnapMode.Intersection
                    | LineSnapMode.Horizontal
                    | LineSnapMode.Vertical,
                    Threshold = 10.0,
                    GuideStyle = new ShapeStyle(new ArgbColor(128, 0, 255, 255), new ArgbColor(128, 0, 255, 255), 2.0, true, true)
                }
            };

            var tools = new ObservableCollection<ToolBase>();

            var noneTool = new NoneTool()
            {
                Settings = new NoneToolSettings()
            };

            var selectionTool = new SelectionTool()
            {
                Filters = new List<PointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = new ShapeStyle(new ArgbColor(128, 0, 255, 255), new ArgbColor(128, 0, 255, 255), 2.0, true, true)
                        }
                    }
                },
                Settings = new SelectionToolSettings()
                {
                    Mode = Core2D.Editor.Selection.SelectionMode.Point | Core2D.Editor.Selection.SelectionMode.Shape,
                    Targets = SelectionTargets.Shapes | SelectionTargets.Guides,
                    SelectionModifier = Modifier.Control,
                    ConnectionModifier = Modifier.Shift,
                    SelectionStyle = new ShapeStyle(new ArgbColor(255, 0, 120, 215), new ArgbColor(60, 170, 204, 238), 2.0, true, true),
                    ClearSelectionOnClean = false,
                    HitTestRadius = 7.0,
                    ConnectPoints = true,
                    ConnectTestRadius = 10.0,
                    DisconnectPoints = true,
                    DisconnectTestRadius = 10.0
                }
            };

            var guideTool = new GuideTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new GuideToolSettings()
                {
                    GuideStyle = new ShapeStyle(new ArgbColor(128, 0, 255, 255), new ArgbColor(128, 0, 255, 255), 2.0, true, true)
                }
            };

            var pointTool = new PointTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new PointToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var lineTool = new LineTool()
            {
                Intersections = new List<PointIntersection>
                {
                    new LineLineIntersection()
                    {
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Settings = new EllipseLineSettings()
                        {
                            IsEnabled = true
                        }
                    }
                },
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new LineToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    SplitIntersections = false
                }
            };

            var polyLineTool = new PolyLineTool()
            {
                Intersections = new List<PointIntersection>
                {
                    new LineLineIntersection()
                    {
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Settings = new EllipseLineSettings()
                        {
                            IsEnabled = true
                        }
                    }
                },
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new PolyLineToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var cubicBezierTool = new CubicBezierTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new CubicBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var quadraticBezierTool = new QuadraticBezierTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new QuadraticBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var pathTool = new PathTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new PathToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    FillRule = PathFillRule.EvenOdd,
                    IsFilled = true,
                    IsClosed = true
                }
            };

            pathTool.Settings.Tools = new ObservableCollection<ToolBase>
            {
                new LineTool(),
                new CubicBezierTool(),
                new QuadraticBezierTool(),
                new MoveTool(pathTool)
            };
            pathTool.Settings.CurrentTool = pathTool.Settings.Tools[0];

            var scribbleTool = new ScribbleTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter
                },
                Settings = new ScribbleToolSettings()
                {
                    Simplify = true,
                    Epsilon = 1.0,
                    FillRule = PathFillRule.EvenOdd,
                    IsFilled = false,
                    IsClosed = false
                }
            };

            var rectangleTool = new RectangleTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new RectangleToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var ellipseTool = new EllipseTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new EllipseToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var textTool = new TextTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new TextToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            tools.Add(noneTool);
            tools.Add(selectionTool);
            tools.Add(guideTool);
            tools.Add(pointTool);
            tools.Add(lineTool);
            tools.Add(polyLineTool);
            tools.Add(cubicBezierTool);
            tools.Add(quadraticBezierTool);
            tools.Add(pathTool);
            tools.Add(scribbleTool);
            tools.Add(rectangleTool);
            tools.Add(ellipseTool);
            tools.Add(textTool);

            var currentTool = tools.FirstOrDefault(t => t.Title == "Selection");

            var presenter = new DefaultShapePresenter()
            {
                Helpers = new Dictionary<Type, ShapeHelper>
                {
                    { typeof(PointShape), new PointHelper() },
                    { typeof(LineShape), new LineHelper() },
                    { typeof(CubicBezierShape), new CubicBezierHelper() },
                    { typeof(QuadraticBezierShape), new QuadraticBezierHelper() },
                    { typeof(PathShape), new PathHelper() },
                    { typeof(RectangleShape), new RectangleHelper() },
                    { typeof(EllipseShape), new EllipseHelper() },
                    { typeof(TextShape), new TextHelper() }
                }
            };

            return new MainViewModel()
            {
                Tools = tools,
                CurrentTool = currentTool,
                Mode = EditMode.Mouse,
                Presenter = presenter,
                Renderer = null,
                HitTest = hitTest,
                CurrentContainer = null,
                WorkingContainer = null,
                CurrentStyle = null,
                PointShape = null,
                Capture = null,
                Release = null,
                Invalidate = null,
                Reset = null,
                AutoFit = null,
                StretchNone = null,
                StretchFill = null,
                StretchUniform = null,
                StretchUniformToFill = null
            };
        }

        public void CreateDemoContainer(MainViewModel vm)
        {
            var container = new CanvasContainer()
            {
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 128, 128, 128),
                InputBackground = new ArgbColor(255, 211, 211, 211)
            };

            var workingContainer = new CanvasContainer();

            var style = new ShapeStyle(new ArgbColor(255, 0, 255, 0), new ArgbColor(80, 0, 255, 0), 2.0, true, true);

            var pointShape = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
            {
                Style = new ShapeStyle(new ArgbColor(0, 0, 0, 0), new ArgbColor(255, 255, 255, 0), 2.0, true, true)
            };

            var guideTool = vm.Tools.FirstOrDefault(t => t.Title == "Guide") as GuideTool;

            container.Styles.Add(guideTool.Settings.GuideStyle);
            container.Styles.Add(pointShape.Style);
            container.Styles.Add(style);

            vm.CurrentContainer = container;
            vm.WorkingContainer = workingContainer;
            vm.CurrentStyle = style;
            vm.PointShape = pointShape;
        }

        public void AddDemoGroupShape(IToolContext context)
        {
            var group = new GroupShape();
            group.Shapes.Add(new RectangleShape(
                new PointShape(30, 30, context.PointShape),
                new PointShape(60, 60, context.PointShape))
            {
                Style = context.CurrentStyle
            });
            group.Points.Add(new PointShape(45, 30, context.PointShape));
            group.Points.Add(new PointShape(45, 60, context.PointShape));
            group.Points.Add(new PointShape(30, 45, context.PointShape));
            group.Points.Add(new PointShape(60, 45, context.PointShape));
            context.CurrentContainer.Shapes.Add(group);
        }
    }
}
