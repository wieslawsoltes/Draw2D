// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Filters;
using Draw2D.ViewModels.Intersections;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Editor
{
    public class ContainerEditor : ToolContext
    {
        public static void CreateDemoGroup(IToolContext context)
        {
            var container = context.ContainerView?.CurrentContainer;
            var group = new GroupShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Shapes = new ObservableCollection<BaseShape>()
            };
            group.Shapes.Add(
                new RectangleShape(new PointShape(30, 30, context.ContainerView?.PointTemplate),
                new PointShape(60, 60, context.ContainerView?.PointTemplate))
                {
                    Points = new ObservableCollection<PointShape>(),
                    Style = context.ContainerView?.CurrentStyle
                });
            group.Points.Add(new PointShape(45, 30, context.ContainerView?.PointTemplate));
            group.Points.Add(new PointShape(45, 60, context.ContainerView?.PointTemplate));
            group.Points.Add(new PointShape(30, 45, context.ContainerView?.PointTemplate));
            group.Points.Add(new PointShape(60, 45, context.ContainerView?.PointTemplate));
            context.ContainerView?.CurrentContainer.Shapes.Add(group);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
        }

        private IHitTest _hitTest;
        private ISelection _selection;
        private IDictionary<Type, IShapeDecorator> _decorators;

        public IList<string> Files { get; set; }

        public ContainerEditor()
        {
            Initialize();
            Files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.json").Select(x => Path.GetFileName(x)).ToList();
        }

        private T LoadFromJson<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.FromJson<T>(json);
        }

        private void SaveAsjson<T>(string path, T value)
        {
            var json = JsonSerializer.ToJson<T>(value);
            File.WriteAllText(path, json);
        }

        private void Initialize()
        {
            var hitTest = new HitTest()
            {
                Registered = new Dictionary<Type, IBounds>()
            };

            hitTest.Register(new PointBounds());
            hitTest.Register(new LineBounds());
            hitTest.Register(new CubicBezierBounds());
            hitTest.Register(new QuadraticBezierBounds());
            hitTest.Register(new ConicBounds());
            hitTest.Register(new GroupBounds());
            hitTest.Register(new PathBounds());
            hitTest.Register(new RectangleBounds());
            hitTest.Register(new EllipseBounds());
            hitTest.Register(new TextBounds());

            var gridSnapPointFilter = new GridSnapPointFilter()
            {
                Guides = new ObservableCollection<BaseShape>(),
                Settings = new GridSnapSettings()
                {
                    IsEnabled = true,
                    EnableGuides = false,
                    Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                    GridSizeX = 15.0,
                    GridSizeY = 15.0,
                    GuideStyle = new ShapeStyle(
                        new ArgbColor(128, 0, 255, 255),
                        new ArgbColor(128, 0, 255, 255),
                        2.0, true, true,
                        new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(128, 0, 255, 255), true))
                }
            };

            var lineSnapPointFilter = new LineSnapPointFilter()
            {
                Guides = new ObservableCollection<BaseShape>(),
                Settings = new LineSnapSettings()
                {
                    IsEnabled = true,
                    EnableGuides = false,
                    Target = LineSnapTarget.Shapes,
                    Mode = LineSnapMode.Point
                    | LineSnapMode.Middle
                    | LineSnapMode.Nearest
                    | LineSnapMode.Intersection
                    | LineSnapMode.Horizontal
                    | LineSnapMode.Vertical,
                    Threshold = 10.0,
                    GuideStyle = new ShapeStyle(
                        new ArgbColor(128, 0, 255, 255),
                        new ArgbColor(128, 0, 255, 255),
                        2.0, true, true,
                        new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(128, 0, 255, 255), true))
                }
            };

            var tools = new ObservableCollection<ITool>();

            var noneTool = new NoneTool()
            {
                Settings = new NoneToolSettings()
            };

            var selectionTool = new SelectionTool()
            {
                Filters = new ObservableCollection<PointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<BaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = new ShapeStyle(
                                new ArgbColor(128, 0, 255, 255),
                                new ArgbColor(128, 0, 255, 255),
                                2.0, true, true,
                                new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(128, 0, 255, 255), true))
                        }
                    }
                },
                Settings = new SelectionToolSettings()
                {
                    Mode = ViewModels.Tools.SelectionMode.Point | ViewModels.Tools.SelectionMode.Shape,
                    Targets = ViewModels.Tools.SelectionTargets.Shapes,
                    SelectionModifier = Modifier.Control,
                    ConnectionModifier = Modifier.Shift,
                    SelectionStyle = new ShapeStyle(
                        new ArgbColor(255, 0, 120, 215),
                        new ArgbColor(60, 170, 204, 238),
                        2.0, true, true,
                        new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 120, 215), true)),
                    ClearSelectionOnClean = false,
                    HitTestRadius = 7.0,
                    ConnectPoints = true,
                    ConnectTestRadius = 10.0,
                    DisconnectPoints = true,
                    DisconnectTestRadius = 10.0
                },
                Hovered = null,
                Selected = new HashSet<BaseShape>()
            };

            var pointTool = new PointTool()
            {
                Filters = new ObservableCollection<PointFilter>
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
                Intersections = new ObservableCollection<PointIntersection>
                {
                    new LineLineIntersection()
                    {
                        Intersections = new ObservableCollection<PointShape>(),
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Intersections = new ObservableCollection<PointShape>(),
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Intersections = new ObservableCollection<PointShape>(),
                        Settings = new EllipseLineSettings()
                        {
                            IsEnabled = true
                        }
                    }
                },
                Filters = new ObservableCollection<PointFilter>
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
                Intersections = new ObservableCollection<PointIntersection>
                {
                    new LineLineIntersection()
                    {
                        Intersections = new ObservableCollection<PointShape>(),
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Intersections = new ObservableCollection<PointShape>(),
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Intersections = new ObservableCollection<PointShape>(),
                        Settings = new EllipseLineSettings()
                        {
                            IsEnabled = true
                        }
                    }
                },
                Filters = new ObservableCollection<PointFilter>
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
                Filters = new ObservableCollection<PointFilter>
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
                Filters = new ObservableCollection<PointFilter>
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

            var conicTool = new ConicTool()
            {
                Filters = new ObservableCollection<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new ConicToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    Weight = 1.0
                }
            };

            var pathTool = new PathTool()
            {
                Filters = new ObservableCollection<PointFilter>
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

            pathTool.Settings.Tools = new ObservableCollection<ITool>
            {
                new LineTool(),
                new CubicBezierTool(),
                new QuadraticBezierTool(),
                new ConicTool(),
                new MoveTool(pathTool)
            };
            pathTool.Settings.CurrentTool = pathTool.Settings.Tools[0];

            var scribbleTool = new ScribbleTool()
            {
                Filters = new ObservableCollection<PointFilter>
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
                Filters = new ObservableCollection<PointFilter>
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
                Filters = new ObservableCollection<PointFilter>
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
                Filters = new ObservableCollection<PointFilter>
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
            tools.Add(pointTool);
            tools.Add(lineTool);
            tools.Add(polyLineTool);
            tools.Add(cubicBezierTool);
            tools.Add(quadraticBezierTool);
            tools.Add(conicTool);
            tools.Add(pathTool);
            tools.Add(scribbleTool);
            tools.Add(rectangleTool);
            tools.Add(ellipseTool);
            tools.Add(textTool);

            var currentTool = tools.FirstOrDefault(t => t.Title == "Selection");

            var decorators = new Dictionary<Type, IShapeDecorator>
            {
                //{ typeof(PointShape), new PointDecorator() },
                { typeof(LineShape), new LineDecorator() },
                { typeof(CubicBezierShape), new CubicBezierDecorator() },
                { typeof(QuadraticBezierShape), new QuadraticBezierDecorator() },
                { typeof(ConicShape), new ConicDecorator() },
                { typeof(PathShape), new PathDecorator() },
                { typeof(RectangleShape), new RectangleDecorator() },
                { typeof(EllipseShape), new EllipseDecorator() },
                { typeof(TextShape), new TextDecorator() }
            };

            _decorators = decorators;

            var containerViews = new ObservableCollection<IContainerView>();

            _hitTest = hitTest;
            _selection = selectionTool;

            containerViews.Add(CreateContainerView("View0"));
            containerViews.Add(CreateContainerView("View1"));
            containerViews.Add(CreateContainerView("View2"));

            ContainerViews = containerViews;
            ContainerView = containerViews.FirstOrDefault();
            Tools = tools;
            CurrentTool = currentTool;
            Mode = EditMode.Mouse;
        }

        private ICanvasContainer CreateCurrentCanvasContainer()
        {
            return new CanvasContainer()
            {
                Shapes = new ObservableCollection<BaseShape>()
            };
        }

        private ICanvasContainer CreateWorkingCanvasContainer()
        {
            return new CanvasContainer()
            {
                Shapes = new ObservableCollection<BaseShape>()
            };
        }

        public IContainerView CreateContainerView(string title)
        {
            var containerView = new ContainerView()
            {
                Title = title,
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 128, 128, 128),
                InputBackground = new ArgbColor(255, 211, 211, 211),
                Styles = new ObservableCollection<ShapeStyle>(),
                CurrentStyle = new ShapeStyle(
                                new ArgbColor(255, 0, 255, 0),
                                new ArgbColor(80, 0, 255, 0),
                                2.0, true, true,
                                new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 0), true)),
                PointTemplate = new EllipseShape(new PointShape(-3, -3, null), new PointShape(3, 3, null))
                {
                    Points = new ObservableCollection<PointShape>(),
                    Style = new ShapeStyle(
                                    new ArgbColor(255, 255, 255, 0),
                                    new ArgbColor(255, 255, 255, 0),
                                    2.0, true, true,
                                    new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 255, 0), true))
                },
                InputService = null,
                DrawContainerView = new DrawContainerView()
                {
                    Decorators = _decorators
                },
                Selection = _selection,
                CurrentContainer = null,
                WorkingContainer = null,
                HitTest = _hitTest
            };

            containerView.CurrentContainer = CreateCurrentCanvasContainer();

            containerView.WorkingContainer = CreateWorkingCanvasContainer();

            return containerView;
        }

        public void CloseContainerView(IContainerView containerView)
        {
            if (containerView != null)
            {
                int index = ContainerViews.IndexOf(containerView);
                if (index >= 0)
                {
                    ContainerViews.Remove(containerView);
                    containerView.DrawContainerView.Dispose();

                    int count = ContainerViews.Count;
                    if (count > 0)
                    {
                        int selectedIndex = (count == 1 || index == 0) ? 0 : index - 1;
                        ContainerView = ContainerViews[selectedIndex];
                    }
                    else
                    {
                        ContainerView = null;
                    }
                }
            }
        }

        private void AddContainerView(IContainerView containerView)
        {
            CurrentTool.Clean(this);
            ContainerView?.Selection.Selected.Clear();

            ContainerViews.Add(containerView);
            ContainerView = containerView;

            ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void NewContainerView()
        {
            var containerView = CreateContainerView("View" + ContainerViews.Count.ToString());

            AddContainerView(containerView);
        }

        public void Open(string path)
        {
            var containerView = LoadFromJson<ContainerView>(path);
            containerView.DrawContainerView = new DrawContainerView()
            {
                Decorators = _decorators
            };
            containerView.Selection = _selection;
            containerView.HitTest = _hitTest;
            containerView.WorkingContainer = CreateWorkingCanvasContainer();

            AddContainerView(containerView);
        }

        public void Save(string path)
        {
            SaveAsjson(path, ContainerView);
        }

        public async void OpenContainerView()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                Open(path);
            }
        }

        public async void SaveContainerViewAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = "container";
            dlg.DefaultExtension = "json";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                Save(path);
            }
        }

        public PointShape GetFirstPoint(PathShape path)
        {
            if (path?.Figures.Count > 0)
            {
                var shapes = path.Figures[path.Figures.Count - 1].Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[0])
                    {
                        case LineShape line:
                            return line.StartPoint;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.StartPoint;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.StartPoint;
                        case ConicShape conic:
                            return conic.StartPoint;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public PointShape GetLastPoint(PathShape path)
        {
            if (path?.Figures.Count > 0)
            {
                var shapes = path.Figures[path.Figures.Count - 1].Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[shapes.Count - 1])
                    {
                        case LineShape line:
                            return line.Point;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.Point3;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.Point2;
                        case ConicShape conic:
                            return conic.Point2;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public PathShape ConvertSKPath(SKPath path)
        {
            var pathShape = new PathShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Figures = new ObservableCollection<FigureShape>(),
                FillRule = PathFillRule.EvenOdd,
                Style = ContainerView.CurrentStyle
            };

            var figureShape = default(FigureShape);

            using (var iterator = path.CreateRawIterator())
            {
                var points = new SKPoint[4];
                var pathVerb = SKPathVerb.Move;
                var firstPoint = new SKPoint();
                var lastPoint = new SKPoint();

                while ((pathVerb = iterator.Next(points)) != SKPathVerb.Done)
                {
                    switch (pathVerb)
                    {
                        case SKPathVerb.Move:
                            {
                                figureShape = new FigureShape()
                                {
                                    Shapes = new ObservableCollection<BaseShape>(),
                                    IsFilled = true,
                                    IsClosed = false
                                };
                                pathShape.Figures.Add(figureShape);
                                firstPoint = lastPoint = points[0];
                            }
                            break;
                        case SKPathVerb.Line:
                            {
                                var lastPointShape = GetLastPoint(pathShape);
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, ContainerView.PointTemplate);
                                }
                                var lineShape = new LineShape()
                                {
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point = new PointShape(points[1].X, points[1].Y, ContainerView.PointTemplate),
                                    Style = ContainerView.CurrentStyle
                                };
                                figureShape.Shapes.Add(lineShape);
                                lastPoint = points[1];
                            }
                            break;
                        case SKPathVerb.Cubic:
                            {
                                var lastPointShape = GetLastPoint(pathShape);
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, ContainerView.PointTemplate);
                                }
                                var cubicBezierShape = new CubicBezierShape()
                                {
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, ContainerView.PointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, ContainerView.PointTemplate),
                                    Point3 = new PointShape(points[3].X, points[3].Y, ContainerView.PointTemplate),
                                    Style = ContainerView.CurrentStyle
                                };
                                figureShape.Shapes.Add(cubicBezierShape);
                                lastPoint = points[3];
                            }
                            break;
                        case SKPathVerb.Quad:
                            {
                                var lastPointShape = GetLastPoint(pathShape);
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, ContainerView.PointTemplate);
                                }
                                var quadraticBezierShape = new QuadraticBezierShape()
                                {
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, ContainerView.PointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, ContainerView.PointTemplate),
                                    Style = ContainerView.CurrentStyle
                                };
                                figureShape.Shapes.Add(quadraticBezierShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Conic:
                            {
                                var lastPointShape = GetLastPoint(pathShape);
                                if (lastPointShape == null)
                                {
                                    lastPointShape = new PointShape(points[0].X, points[0].Y, ContainerView.PointTemplate);
                                }
                                var quadraticBezierShape = new ConicShape()
                                {
                                    Points = new ObservableCollection<PointShape>(),
                                    StartPoint = lastPointShape,
                                    Point1 = new PointShape(points[1].X, points[1].Y, ContainerView.PointTemplate),
                                    Point2 = new PointShape(points[2].X, points[2].Y, ContainerView.PointTemplate),
                                    Weight = iterator.ConicWeight(),
                                    Style = ContainerView.CurrentStyle
                                };
                                figureShape.Shapes.Add(quadraticBezierShape);
                                lastPoint = points[2];
                            }
                            break;
                        case SKPathVerb.Close:
                            {
                                //var line = new LineShape()
                                //{
                                //    Points = new ObservableCollection<PointShape>(),
                                //    StartPoint = GetLastPoint(pathShape),
                                //    Point = GetFirstPoint(pathShape),
                                //    Style = ContainerView.CurrentStyle
                                //};
                                //figureShape.Shapes.Add(line);
                                figureShape.IsClosed = true;
                                firstPoint = lastPoint = new SKPoint(0, 0);
                            }
                            break;
                    }
                }
            }
            return pathShape;
        }

        public void FromSvgPathData(TextBox textBox)
        {
            var svgPathData = textBox.Text;
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                var path = SKPath.ParseSvgPathData(svgPathData);
                var pathShape = ConvertSKPath(path);
                ContainerView.CurrentContainer.Shapes.Add(pathShape);
                ContainerView.CurrentContainer.MarkAsDirty(true);
                ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void ToSvgPathData(TextBox textBox)
        {
            var selected = ContainerView.Selection.Selected;
            if (selected != null)
            {
                var shape = selected.FirstOrDefault();
                switch (shape)
                {
                    case CubicBezierShape cubicBezierShape:
                        {
                            var path = SkiaHelper.ToGeometry(cubicBezierShape, 0.0, 0.0);
                            var svgPathData = path.ToSvgPathData();
                            textBox.Text = svgPathData;
                            Application.Current.Clipboard.SetTextAsync(svgPathData);
                        }
                        break;
                    case EllipseShape ellipseShape:
                        {
                        }
                        break;
                    case FigureShape figureShape:
                        {
                        }
                        break;
                    case GroupShape groupShape:
                        {
                        }
                        break;
                    case LineShape lineShape:
                        {
                        }
                        break;
                    case PathShape pathShape:
                        {
                            var path = SkiaHelper.ToGeometry(pathShape, 0.0, 0.0);
                            var svgPathData = path.ToSvgPathData();
                            textBox.Text = svgPathData;
                            Application.Current.Clipboard.SetTextAsync(svgPathData);
                        }
                        break;
                    case PointShape pointShape:
                        {
                        }
                        break;
                    case QuadraticBezierShape quadraticBezierShape:
                        {
                            var path = SkiaHelper.ToGeometry(quadraticBezierShape, 0.0, 0.0);
                            var svgPathData = path.ToSvgPathData();
                            textBox.Text = svgPathData;
                            Application.Current.Clipboard.SetTextAsync(svgPathData);
                        }
                        break;
                    case RectangleShape rectangleShape:
                        {
                        }
                        break;
                    case TextShape textShape:
                        {

                        }
                        break;
                };
            }
        }

        public void Exit()
        {
            Application.Current.Windows.FirstOrDefault()?.Close();
        }
    }
}
