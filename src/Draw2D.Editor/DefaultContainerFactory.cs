// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Input;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Filters;
using Draw2D.ViewModels.Intersections;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Style.PathEffects;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor
{
    public class DefaultContainerFactory : IContainerFactory
    {
        public IStyleLibrary CreateStyleLibrary()
        {
            var styleLibrary = new StyleLibrary()
            {
                Items = new ObservableCollection<IShapeStyle>()
            };

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Default",
                    new StrokePaint(color: new ArgbColor(255, 0, 0, 0)),
                    new FillPaint(color: new ArgbColor(255, 255, 255, 255)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Red",
                    new StrokePaint(color: new ArgbColor(255, 255, 0, 0)),
                    new FillPaint(color: new ArgbColor(255, 255, 0, 0)),
                    new TextPaint(color: new ArgbColor(255, 255, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Green",
                    new StrokePaint(color: new ArgbColor(255, 0, 255, 0)),
                    new FillPaint(color: new ArgbColor(255, 0, 255, 0)),
                    new TextPaint(color: new ArgbColor(255, 0, 255, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Blue",
                    new StrokePaint(color: new ArgbColor(255, 0, 0, 255)),
                    new FillPaint(color: new ArgbColor(255, 0, 0, 255)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 255)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Cyan",
                    new StrokePaint(color: new ArgbColor(255, 0, 255, 255)),
                    new FillPaint(color: new ArgbColor(255, 0, 255, 255)),
                    new TextPaint(color: new ArgbColor(255, 0, 255, 255)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Magenta",
                    new StrokePaint(color: new ArgbColor(255, 255, 0, 255)),
                    new FillPaint(color: new ArgbColor(255, 255, 0, 255)),
                    new TextPaint(color: new ArgbColor(255, 255, 0, 255)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Yellow",
                    new StrokePaint(color: new ArgbColor(255, 255, 255, 0)),
                    new FillPaint(color: new ArgbColor(255, 255, 255, 0)),
                    new TextPaint(color: new ArgbColor(255, 255, 255, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Black",
                    new StrokePaint(color: new ArgbColor(255, 0, 0, 0)),
                    new FillPaint(color: new ArgbColor(255, 0, 0, 0)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Dash",
                    new StrokePaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        pathEffect: PathDashEffect.MakeDash()),
                    new FillPaint(color: new ArgbColor(255, 0, 0, 0)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Dot",
                    new StrokePaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        pathEffect: PathDashEffect.MakeDot()),
                    new FillPaint(color: new ArgbColor(255, 0, 0, 0)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "DashDot",
                    new StrokePaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        pathEffect: PathDashEffect.MakeDashDot()),
                    new FillPaint(color: new ArgbColor(255, 0, 0, 0)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Fuzzy",
                    new StrokePaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        pathEffect: PathDiscreteEffect.MakeDiscrete()),
                    new FillPaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        pathEffect: PathDiscreteEffect.MakeDiscrete()),
                    new TextPaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        pathEffect: PathDiscreteEffect.MakeDiscrete()),
                    isStroked: true, isFilled: true, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "DashDotDot",
                    new StrokePaint(
                        color: new ArgbColor(255, 0, 0, 0),
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        pathEffect: PathDashEffect.MakeDashDotDot()),
                    new FillPaint(color: new ArgbColor(255, 0, 0, 0)),
                    new TextPaint(color: new ArgbColor(255, 0, 0, 0)),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Decorator-Stroke",
                    new StrokePaint(color: new ArgbColor(255, 0, 255, 255)),
                    new FillPaint(color: new ArgbColor(255, 0, 255, 255)),
                    new TextPaint(color: new ArgbColor(255, 0, 255, 255)),
                    isStroked: true, isFilled: false, isText: false));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Decorator-Fill",
                    new StrokePaint(color: new ArgbColor(255, 0, 255, 255)),
                    new FillPaint(color: new ArgbColor(255, 0, 255, 255)),
                    new TextPaint(color: new ArgbColor(255, 0, 255, 255)),
                    isStroked: false, isFilled: true, isText: false));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Guide",
                    new StrokePaint(color: new ArgbColor(128, 0, 255, 255)),
                    new FillPaint(color: new ArgbColor(128, 0, 255, 255)),
                    new TextPaint(color: new ArgbColor(128, 0, 255, 255)),
                    isStroked: true, isFilled: true, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Selection",
                    new StrokePaint(color: new ArgbColor(255, 0, 120, 215)),
                    new FillPaint(color: new ArgbColor(60, 170, 204, 238)),
                    new TextPaint(color: new ArgbColor(255, 0, 120, 215)),
                    isStroked: true, isFilled: true, isText: false));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "PointTemplate",
                    new StrokePaint(color: new ArgbColor(255, 255, 0, 255)),
                    new FillPaint(color: new ArgbColor(255, 255, 0, 255)),
                    new TextPaint(color: new ArgbColor(255, 255, 0, 255)),
                    isStroked: false, isFilled: true, isText: false));

            styleLibrary.CurrentItem = styleLibrary.Items[0];

            return styleLibrary;
        }

        public IGroupLibrary CreateGroupLibrary()
        {
            var groupsLibrary = new GroupLibrary()
            {
                Items = new ObservableCollection<GroupShape>()
            };

            groupsLibrary.CurrentItem = null;

            return groupsLibrary;
        }

        public IToolContext CreateToolContext()
        {
            var editorToolContext = new EditorToolContext()
            {
                ContainerFactory = this
            };

            var hitTest = new HitTest();

            var tools = new ObservableCollection<ITool>();

            var noneTool = new NoneTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = false,
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
                    },
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = false,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new NoneToolSettings()
            };

            var selectionTool = new SelectionTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = false,
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
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new PointToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var lineTool = new LineTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>
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
                    new CircleLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new CircleLineSettings()
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
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
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
                Intersections = new ObservableCollection<IPointIntersection>
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
                    new CircleLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new CircleLineSettings()
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
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new PolyLineToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var cubicBezierTool = new CubicBezierTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new CubicBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var quadraticBezierTool = new QuadraticBezierTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new QuadraticBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var conicTool = new ConicTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
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
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new PathToolSettings()
                {
                    Tools = new ObservableCollection<ITool>(),
                    CurrentTool = null,
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    FillType = PathFillType.Winding,
                    IsFilled = true,
                    IsClosed = true
                }
            };

            var pathLineTool = new LineTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>
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
                        new CircleLineIntersection()
                        {
                            Intersections = new ObservableCollection<IPointShape>(),
                            Settings = new CircleLineSettings()
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
                Filters = new ObservableCollection<IPointFilter>
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
                        },
                        new LineSnapPointFilter()
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
                        }
                    },
                Settings = new LineToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    SplitIntersections = false
                }
            };

            var pathCubicBezierTool = new CubicBezierTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new CubicBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var pathQuadraticBezierTool = new QuadraticBezierTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new QuadraticBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var pathConicTool = new ConicTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new ConicToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    Weight = 1.0
                }
            };

            var pathMoveTool = new MoveTool(pathTool)
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>(),
                Settings = new MoveToolSettings()
            };

            var scribbleTool = new ScribbleTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = false,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new ScribbleToolSettings()
                {
                    Simplify = true,
                    Epsilon = 1.0,
                    FillType = PathFillType.Winding,
                    IsFilled = false,
                    IsClosed = false
                }
            };

            var rectangleTool = new RectangleTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new RectangleToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var circleTool = new CircleTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new CircleToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var arcTool = new ArcTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new ArcToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    StartAngle = -180,
                    SweepAngle = 180
                }
            };

            var ellipseTool = new EllipseTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new EllipseToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var textTool = new TextTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
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
                    },
                    new LineSnapPointFilter()
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
                    }
                },
                Settings = new TextToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            void SetToolDefaults(ITool tool)
            {
                tool.CurrentIntersection = tool.Intersections.Count > 0 ? tool.Intersections[0] : null;
                tool.CurrentFilter = tool.Filters.Count > 0 ? tool.Filters[0] : null;
            }

            SetToolDefaults(noneTool);
            SetToolDefaults(selectionTool);
            SetToolDefaults(pointTool);
            SetToolDefaults(lineTool);
            SetToolDefaults(polyLineTool);
            SetToolDefaults(cubicBezierTool);
            SetToolDefaults(quadraticBezierTool);
            SetToolDefaults(conicTool);
            SetToolDefaults(pathTool);
            SetToolDefaults(scribbleTool);
            SetToolDefaults(rectangleTool);
            SetToolDefaults(circleTool);
            SetToolDefaults(arcTool);
            SetToolDefaults(ellipseTool);
            SetToolDefaults(textTool);

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
            tools.Add(circleTool);
            tools.Add(arcTool);
            tools.Add(ellipseTool);
            tools.Add(textTool);

            SetToolDefaults(pathLineTool);
            SetToolDefaults(pathCubicBezierTool);
            SetToolDefaults(pathQuadraticBezierTool);
            SetToolDefaults(pathConicTool);
            SetToolDefaults(pathMoveTool);

            pathTool.Settings.Tools.Add(pathLineTool);
            pathTool.Settings.Tools.Add(pathCubicBezierTool);
            pathTool.Settings.Tools.Add(pathQuadraticBezierTool);
            pathTool.Settings.Tools.Add(pathConicTool);
            pathTool.Settings.Tools.Add(pathMoveTool);

            pathTool.Settings.CurrentTool = pathTool.Settings.Tools[0];

            editorToolContext.Selection = selectionTool;
            editorToolContext.HitTest = hitTest;
            editorToolContext.CurrentDirectory = null;
            editorToolContext.Files = new ObservableCollection<string>();

            editorToolContext.DocumentContainer = null;

            editorToolContext.Tools = tools;
            editorToolContext.CurrentTool = selectionTool;
            editorToolContext.EditMode = EditMode.Mouse;

            return editorToolContext;
        }

        public IContainerView CreateContainerView(string title)
        {
            var containerView = new ContainerView()
            {
                Title = title,
                Width = 720,
                Height = 630,
                PrintBackground = new FillPaint(color: new ArgbColor(0, 255, 255, 255), isAntialias: false),
                WorkBackground = new FillPaint(color: new ArgbColor(255, 255, 255, 255), isAntialias: false),
                InputBackground = new FillPaint(color: new ArgbColor(0, 255, 255, 255), isAntialias: false),
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
                ContainerPresenter = null,
                InputService = null,
                ZoomService = null
            };

            return containerView;
        }

        public IDocumentContainer CreateDocumentContainer(string title)
        {
            var documentContainer = new DocumentContainer()
            {
                Title = title
            };

            documentContainer.StyleLibrary = null;
            documentContainer.GroupLibrary = null;

            var pointTemplate = new RectangleShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = new Text(),
                StyleId = "PointTemplate"
            };
            pointTemplate.StartPoint.Owner = pointTemplate;
            pointTemplate.Point.Owner = pointTemplate;

            documentContainer.PointTemplate = pointTemplate;

            documentContainer.ContainerViews = new ObservableCollection<IContainerView>();
            documentContainer.ContainerView = null;

            return documentContainer;
        }
    }
}
