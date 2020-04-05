using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Input;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Filters;
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
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 0, 0), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Red",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 255, 0, 0), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Green",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 255, 0), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Blue",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 0, 255), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 255), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Cyan",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 255, 255), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Magenta",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 255, 0, 255), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 0, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 0, 255), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Yellow",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 255, 255, 0), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 255, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 255, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Black",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 0, 0), isScaled: false, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Dash",
                    strokePaint: new Paint(
                        paintStyle: PaintStyle.Stroke,
                        color: new ArgbColor(255, 0, 0, 0),
                        isScaled: false,
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        effects: new PaintEffects(pathEffect: PathDashEffect.MakeDash())),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Dot",
                    strokePaint: new Paint(
                        paintStyle: PaintStyle.Stroke,
                        color: new ArgbColor(255, 0, 0, 0),
                        isScaled: false,
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        effects: new PaintEffects(pathEffect: PathDashEffect.MakeDot())),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "DashDot",
                    strokePaint: new Paint(
                        paintStyle: PaintStyle.Stroke,
                        color: new ArgbColor(255, 0, 0, 0),
                        isScaled: false,
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        effects: new PaintEffects(pathEffect: PathDashEffect.MakeDashDot())),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "DashDotDot",
                    strokePaint: new Paint(
                        paintStyle: PaintStyle.Stroke,
                        color: new ArgbColor(255, 0, 0, 0),
                        isScaled: false,
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        effects: new PaintEffects(pathEffect: PathDashEffect.MakeDashDotDot())),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 0, 0), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Fuzzy",
                    strokePaint: new Paint(
                        paintStyle: PaintStyle.Stroke,
                        color: new ArgbColor(255, 0, 0, 0),
                        isScaled: false,
                        strokeCap: StrokeCap.Round,
                        strokeJoin: StrokeJoin.Round,
                        effects: new PaintEffects(pathEffect: PathDiscreteEffect.MakeDiscrete())),
                    fillPaint: new Paint(
                        paintStyle: PaintStyle.Fill,
                        color: new ArgbColor(255, 0, 0, 0),
                        effects: new PaintEffects(pathEffect: PathDiscreteEffect.MakeDiscrete())),
                    textPaint: new Paint(
                        paintStyle: PaintStyle.Fill,
                        color: new ArgbColor(255, 0, 0, 0),
                        effects: new PaintEffects(pathEffect: PathDiscreteEffect.MakeDiscrete())),
                    isStroked: true, isFilled: true, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Decorator-Stroke",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 255, 255), isScaled: true, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: false, isText: false));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Decorator-Fill",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 255, 255), isScaled: true, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    isStroked: false, isFilled: true, isText: false));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Guide",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(128, 0, 255, 255), isScaled: true, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(128, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(128, 0, 255, 255), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: true, isText: true));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Selection",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 0, 120, 215), isScaled: true, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(60, 170, 204, 238), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 0, 120, 215), effects: PaintEffects.MakeEffects()),
                    isStroked: true, isFilled: true, isText: false));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "PointTemplate",
                    strokePaint: new Paint(paintStyle: PaintStyle.Stroke, color: new ArgbColor(255, 255, 0, 255), isScaled: true, effects: PaintEffects.MakeEffects()),
                    fillPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 0, 255), effects: PaintEffects.MakeEffects()),
                    textPaint: new Paint(paintStyle: PaintStyle.Fill, color: new ArgbColor(255, 255, 0, 255), effects: PaintEffects.MakeEffects()),
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
                    CopyModifier = Modifier.Alt,
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
                    HitTestRadius = 7.0
                }
            };

            var polyLineTool = new PolyLineTool()
            {
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
                    HitTestRadius = 7.0
                }
            };

            var pathCubicBezierTool = new CubicBezierTool()
            {
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
                Filters = new ObservableCollection<IPointFilter>(),
                Settings = new MoveToolSettings()
            };

            var scribbleTool = new ScribbleTool()
            {
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

            var ovalTool = new OvalTool()
            {
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
                Settings = new OvalToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var textTool = new TextTool()
            {
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
            SetToolDefaults(ovalTool);
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
            tools.Add(ovalTool);
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
                PrintBackground = new Paint(
                    paintStyle: PaintStyle.Fill,
                    color: new ArgbColor(255, 255, 255, 255),
                    isAntialias: false,
                    effects: PaintEffects.MakeEffects()),
                WorkBackground = new Paint(
                    paintStyle: PaintStyle.Fill,
                    color: new ArgbColor(255, 255, 255, 255),
                    isAntialias: false,
                    effects: PaintEffects.MakeEffects()),
                InputBackground = new Paint(
                    paintStyle: PaintStyle.Fill,
                    color: new ArgbColor(0, 255, 255, 255),
                    isAntialias: false,
                    effects: PaintEffects.MakeEffects()),
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
