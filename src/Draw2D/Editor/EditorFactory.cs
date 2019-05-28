// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Filters;
using Draw2D.ViewModels.Intersections;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor
{
    public class EditorFactory : IFactory
    {
        public IStyleLibrary CreateStyleLibrary()
        {
            var styleLibrary = new StyleLibrary()
            {
                Styles = new ObservableCollection<ShapeStyle>()
            };

            var fontFamily = "Calibri";

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Default",
                    new ArgbColor(255, 0, 0, 0),
                    new ArgbColor(255, 255, 255, 255),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 0, 0), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Red",
                    new ArgbColor(255, 255, 0, 0),
                    new ArgbColor(255, 255, 0, 0),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 0, 0), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Green",
                    new ArgbColor(255, 0, 255, 0),
                    new ArgbColor(255, 0, 255, 0),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 0), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Blue",
                    new ArgbColor(255, 0, 0, 255),
                    new ArgbColor(255, 0, 0, 255),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 0, 255), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Cyan",
                    new ArgbColor(255, 0, 255, 255),
                    new ArgbColor(255, 0, 255, 255),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Magenta",
                    new ArgbColor(255, 255, 0, 255),
                    new ArgbColor(255, 255, 0, 255),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 0, 255), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Yellow",
                    new ArgbColor(255, 255, 255, 0),
                    new ArgbColor(255, 255, 255, 0),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 255, 0), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Black",
                    new ArgbColor(255, 0, 0, 0),
                    new ArgbColor(255, 0, 0, 0),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 0, 0), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Decorator-Stroke",
                    new ArgbColor(255, 0, 255, 255),
                    new ArgbColor(255, 0, 255, 255),
                    2.0, true, false,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Decorator-Fill",
                    new ArgbColor(255, 0, 255, 255),
                    new ArgbColor(255, 0, 255, 255),
                    2.0, false, true,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Guide",
                    new ArgbColor(128, 0, 255, 255),
                    new ArgbColor(128, 0, 255, 255),
                    2.0, true, true,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(128, 0, 255, 255), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "Selection",
                    new ArgbColor(255, 0, 120, 215),
                    new ArgbColor(60, 170, 204, 238),
                    2.0, true, true,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 120, 215), true)));

            styleLibrary.Styles.Add(
                new ShapeStyle(
                    "PointTemplate",
                    new ArgbColor(255, 255, 0, 255),
                    new ArgbColor(255, 255, 0, 255),
                    2.0, false, true,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 0, 255), true)));

            styleLibrary.CurrentStyle = styleLibrary.Styles[0];

            return styleLibrary;
        }

        public IGroupLibrary CreateGroupLibrary()
        {
            var groupsLibrary = new GroupLibrary()
            {
                Groups = new ObservableCollection<GroupShape>()
            };

            groupsLibrary.CurrentGroup = null;

            return groupsLibrary;
        }

        public IToolContext CreateToolContext()
        {
            var editorToolContext = new EditorToolContext(this);

            var hitTest = new HitTest();

            var gridSnapPointFilter = new GridSnapPointFilter()
            {
                Guides = new ObservableCollection<IBaseShape>(),
                Settings = new GridSnapSettings()
                {
                    IsEnabled = true,
                    EnableGuides = false,
                    Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                    GridSizeX = 15.0,
                    GridSizeY = 15.0,
                    GuideStyle = "Guide"
                }
            };

            var lineSnapPointFilter = new LineSnapPointFilter()
            {
                Guides = new ObservableCollection<IBaseShape>(),
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
                    GuideStyle = "Guide"
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
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new SelectionToolSettings()
                {
                    Mode = SelectionMode.Point | SelectionMode.Shape,
                    Targets = SelectionTargets.Shapes,
                    SelectionModifier = Modifier.Control,
                    ConnectionModifier = Modifier.Shift,
                    SelectionStyle = "Selection",
                    ClearSelectionOnClean = false,
                    HitTestRadius = 7.0,
                    ConnectPoints = true,
                    ConnectTestRadius = 10.0,
                    DisconnectPoints = true,
                    DisconnectTestRadius = 10.0
                }
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
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
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
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
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

            editorToolContext.Selection = selectionTool;
            editorToolContext.HitTest = hitTest;
            editorToolContext.CurrentDirectory = null;
            editorToolContext.Files = new ObservableCollection<string>();

            editorToolContext.StyleLibrary = null;
            editorToolContext.GroupLibrary = null;

            var pointTemplate = new RectangleShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = new Text(),
                StyleId = "PointTemplate"
            };
            pointTemplate.TopLeft.Owner = pointTemplate;
            pointTemplate.BottomRight.Owner = pointTemplate;

            editorToolContext.PointTemplate = pointTemplate;

            editorToolContext.ContainerViews = new ObservableCollection<IContainerView>();
            editorToolContext.ContainerView = null;
            editorToolContext.Tools = tools;
            editorToolContext.CurrentTool = selectionTool;
            editorToolContext.Mode = EditMode.Mouse;

            return editorToolContext;
        }

        public IContainerView CreateContainerView(string title)
        {
            var containerView = new ContainerView()
            {
                Title = title,
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 255, 255, 255),
                InputBackground = new ArgbColor(0, 255, 255, 255),
                CurrentContainer = new CanvasContainer()
                {
                    Points = new ObservableCollection<IPointShape>(),
                    Shapes = new ObservableCollection<IBaseShape>()
                },
                WorkingContainer = null,
                SelectionState = new SelectionState()
                {
                    Hovered = null,
                    Selected = null,
                    Shapes = new HashSet<IBaseShape>()
                },
                ZoomServiceState = new ZoomServiceState()
                {
                    ZoomSpeed = 1.2,
                    ZoomX = double.NaN,
                    ZoomY = double.NaN,
                    OffsetX = double.NaN,
                    OffsetY = double.NaN,
                    IsPanning = false,
                    IsZooming = false,
                    InitFitMode = FitMode.Center,
                    AutoFitMode = FitMode.None
                },
                DrawContainerView = null,
                InputService = null,
                ZoomService = null
            };

            return containerView;
        }
    }
}
