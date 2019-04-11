// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Markup.Xaml;
using Draw2D.Editor;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Filters;
using Draw2D.ViewModels.Intersections;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Draw2D.Views;

namespace Draw2D
{
    public class App : Application
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            var window = new MainWindow
            {
                DataContext = CreateDemoEditor(),
            };
            app.Run(window);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToDebug();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static CanvasEditor CreateDemoEditor()
        {
            var hitTest = new HitTest();

            hitTest.Register(new PointBounds());
            hitTest.Register(new LineBounds());
            hitTest.Register(new CubicBezierBounds());
            hitTest.Register(new QuadraticBezierBounds());
            hitTest.Register(new GroupBounds());
            hitTest.Register(new PathBounds());
            hitTest.Register(new RectangleBounds());
            hitTest.Register(new EllipseBounds());
            hitTest.Register(new TextBounds());

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

            var tools = new ObservableCollection<ITool>();

            var noneTool = new NoneTool()
            {
                Settings = new NoneToolSettings()
            };

            var selectionTool = new SelectionTool()
            {
                Filters = new ObservableCollection<PointFilterBase>
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
                    Mode = ViewModels.Tools.SelectionMode.Point | ViewModels.Tools.SelectionMode.Shape,
                    Targets = ViewModels.Tools.SelectionTargets.Shapes | ViewModels.Tools.SelectionTargets.Guides,
                    SelectionModifier = Modifier.Control,
                    ConnectionModifier = Modifier.Shift,
                    SelectionStyle = new ShapeStyle(new ArgbColor(255, 0, 120, 215), new ArgbColor(60, 170, 204, 238), 2.0, true, true),
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

            var guideTool = new GuideTool()
            {
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Intersections = new ObservableCollection<PointIntersectionBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Intersections = new ObservableCollection<PointIntersectionBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                new MoveTool(pathTool)
            };
            pathTool.Settings.CurrentTool = pathTool.Settings.Tools[0];

            var scribbleTool = new ScribbleTool()
            {
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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
                Filters = new ObservableCollection<PointFilterBase>
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

            var presenter = new CanvasPresenter()
            {
                Decorators = new Dictionary<Type, IShapeDecorator>
                {
                    { typeof(PointShape), new PointDecorator() },
                    { typeof(LineShape), new LineDecorator() },
                    { typeof(CubicBezierShape), new CubicBezierDecorator() },
                    { typeof(QuadraticBezierShape), new QuadraticBezierDecorator() },
                    { typeof(PathShape), new PathDecorator() },
                    { typeof(RectangleShape), new RectangleDecorator() },
                    { typeof(EllipseShape), new EllipseDecorator() },
                    { typeof(TextShape), new TextDecorator() }
                }
            };

            var renderer = new AvaloniaShapeRenderer()
            {
                Name = "Default",
                Selection = selectionTool
            };

            var vm = new CanvasEditor()
            {
                // IToolContext
                Renderer = renderer,
                HitTest = hitTest,
                CurrentContainer = null,
                WorkingContainer = null,
                CurrentStyle = null,
                PointShape = null,
                Capture = null,
                Release = null,
                Invalidate = null,
                // ViewModel
                Tools = tools,
                CurrentTool = currentTool,
                Mode = EditMode.Mouse,
                Presenter = presenter,
                Selection = selectionTool,
                Zoom = null
            };

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

            container.Styles.Add(guideTool.Settings.GuideStyle);
            container.Styles.Add(pointShape.Style);
            container.Styles.Add(style);

            vm.CurrentContainer = container;
            vm.WorkingContainer = workingContainer;
            vm.CurrentStyle = style;
            vm.PointShape = pointShape;

            return vm;
        }

        public static void CreateDemoGroup(IToolContext context)
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
