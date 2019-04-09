// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.MatrixExtensions;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Draw2D.Containers;
using Draw2D.Editor;
using Draw2D.Editor.Bounds;
using Draw2D.Editor.Bounds.Shapes;
using Draw2D.Editor.Filters;
using Draw2D.Editor.Intersections;
using Draw2D.Editor.Tools;
using Draw2D.Editor.Decorators;
using Draw2D.Presenters;
using Draw2D.Renderer;
using Draw2D.Shapes;
using Draw2D.Style;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Spatial;

namespace Draw2D
{
    public class NewtonsoftJsonSerializer
    {
        public class CoreContractResolver : DefaultContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                if (objectType.GetInterfaces().Any(i => i == typeof(IDictionary) ||
                   (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>))))
                {
                    return base.CreateArrayContract(objectType);
                }
                return base.CreateContract(objectType);
            }

            public override JsonContract ResolveContract(Type type)
            {
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return base
                        .ResolveContract(typeof(ObservableCollection<>)
                        .MakeGenericType(type.GenericTypeArguments[0]));
                }
                return base.ResolveContract(type);
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization).Where(p => p.Writable).ToList();
            }
        }

        private static readonly JsonSerializerSettings Settings;

        static NewtonsoftJsonSerializer()
        {
            Settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                ContractResolver = new CoreContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Converters = { new KeyValuePairConverter() }
            };
        }

        public static string ToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }
    }

    public abstract class ToolContext : ObservableObject, IToolContext
    {
        private IShapeRenderer _renderer;
        private IHitTest _hitTest;
        private CanvasContainer _currentContainer;
        private CanvasContainer _workingContainer;
        private ShapeStyle _currentStyle;
        private BaseShape _pointShape;

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

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }

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
    }

    public class MainViewModel : ToolContext
    {
        private ObservableCollection<ToolBase> _tools;
        private ToolBase _currentTool;
        private EditMode _mode;
        private ShapePresenter _presenter;
        private IEdit _edit;

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

        public IEdit Edit
        {
            get => _edit;
            set => Update(ref _edit, value);
        }

        public Action Reset { get; set; }

        public Action AutoFit { get; set; }

        public Action StretchNone { get; set; }

        public Action StretchFill { get; set; }

        public Action StretchUniform { get; set; }

        public Action StretchUniformToFill { get; set; }

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

        public T Load<T>(string path)
        {
            var json = File.ReadAllText(path);
            return NewtonsoftJsonSerializer.FromJson<T>(json);
        }

        public void Save<T>(string path, T value)
        {
            var json = NewtonsoftJsonSerializer.ToJson<T>(value);
            File.WriteAllText(path, json);
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

        public async void Open()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                var container = Load<CanvasContainer>(path);
                var workingContainer = new CanvasContainer();
                CurrentTool.Clean(this);
                Renderer.Selected.Clear();
                CurrentContainer = container;
                WorkingContainer = workingContainer;
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
                Save(path, CurrentContainer);
            }
        }

        public void Exit()
        {
            Application.Current.Windows.FirstOrDefault()?.Close();
        }
    }

    public class MainViewModelFactory
    {
        public MainViewModel Create()
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
                    Mode = Draw2D.Editor.Tools.SelectionMode.Point | Draw2D.Editor.Tools.SelectionMode.Shape,
                    Targets = Draw2D.Editor.Tools.SelectionTargets.Shapes | Draw2D.Editor.Tools.SelectionTargets.Guides,
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

            var presenter = new ShapePresenter()
            {
                Decorators = new Dictionary<Type, ShapeDecorator>
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

            var renderer = new AvaloniaShapeRenderer();

            var vm = new MainViewModel()
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
                Edit = selectionTool,
                Reset = null,
                AutoFit = null,
                StretchNone = null,
                StretchFill = null,
                StretchUniform = null,
                StretchUniformToFill = null
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

    public struct AvaloniaBrushCache : IDisposable
    {
        public readonly Brush Stroke;
        public readonly Pen StrokePen;
        public readonly Brush Fill;

        public AvaloniaBrushCache(Brush stroke, Pen strokePen, Brush fill)
        {
            this.Stroke = stroke;
            this.StrokePen = strokePen;
            this.Fill = fill;
        }

        public void Dispose()
        {
        }

        public static Color FromDrawColor(ArgbColor color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static AvaloniaBrushCache FromDrawStyle(ShapeStyle style)
        {
            Brush stroke = null;
            Pen strokePen = null;
            Brush fill = null;

            if (style.Stroke != null)
            {
                stroke = new SolidColorBrush(FromDrawColor(style.Stroke));
                strokePen = new Pen(stroke, style.Thickness);
            }

            if (style.Fill != null)
            {
                fill = new SolidColorBrush(FromDrawColor(style.Fill));
            }

            return new AvaloniaBrushCache(stroke, strokePen, fill);
        }
    }

    public struct FormattedTextCache : IDisposable
    {
        public readonly FormattedText FormattedText;
        public readonly Point Origin;

        public FormattedTextCache(FormattedText formattedText, Point origin)
        {
            FormattedText = formattedText;
            Origin = origin;
        }

        public void Dispose()
        {
        }

        public static FormattedTextCache FromTextShape(TextShape text, Rect rect)
        {
            var constraint = new Size(rect.Width, rect.Height);

            var formattedText = new FormattedText()
            {
                Text = text.Text.Value,
                Constraint = constraint,
                TextAlignment = TextAlignment.Center,
                Wrapping = TextWrapping.NoWrap,
                Typeface = new Typeface("Arial", 11)
            };

            var size = formattedText.Bounds.Size;

            // Vertical Alignment: Top
            //var top = new Point(
            //    rect.X,
            //    rect.Y);

            // Vertical Alignment: Center
            var center = new Point(
                rect.X,
                rect.Y + rect.Height / 2 - size.Height / 2);

            // Vertical Alignment: Bottom
            //var bottom = new Point(
            //    rect.X,
            //    rect.Y + rect.Height - size.Height);

            return new FormattedTextCache(formattedText, center);
        }
    }

    public class AvaloniaShapeRenderer : ObservableObject, IShapeRenderer
    {
        private readonly IDictionary<ShapeStyle, AvaloniaBrushCache> _brushCache;
        private readonly IDictionary<MatrixObject, Matrix> _matrixCache;
        private readonly IDictionary<CubicBezierShape, Geometry> _cubicGeometryCache;
        private readonly IDictionary<QuadraticBezierShape, Geometry> _quadGeometryCache;
        private readonly IDictionary<PathShape, Geometry> _pathGeometryCache;
        private readonly IDictionary<EllipseShape, Geometry> _ellipseGeometryCache;
        private readonly IDictionary<TextShape, FormattedTextCache> _formattedTextCache;

        private BaseShape _hover;
        private ISet<BaseShape> _selected;

        public BaseShape Hover
        {
            get => _hover;
            set => Update(ref _hover, value);
        }

        public ISet<BaseShape> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public AvaloniaShapeRenderer()
        {
            _brushCache = new Dictionary<ShapeStyle, AvaloniaBrushCache>();
            _formattedTextCache = new Dictionary<TextShape, FormattedTextCache>();
            _matrixCache = new Dictionary<MatrixObject, Matrix>();
            _cubicGeometryCache = new Dictionary<CubicBezierShape, Geometry>();
            _quadGeometryCache = new Dictionary<QuadraticBezierShape, Geometry>();
            _pathGeometryCache = new Dictionary<PathShape, Geometry>();
            _ellipseGeometryCache = new Dictionary<EllipseShape, Geometry>();
            _hover = null;
            _selected = new HashSet<BaseShape>();
        }

        private static Point ToPoint(PointShape point, double dx, double dy)
        {
            return new Point(point.X + dx, point.Y + dy);
        }

        public static IEnumerable<Point> ToPoints(IEnumerable<PointShape> points, double dx, double dy)
        {
            return points.Select(point => new Point(point.X + dx, point.Y + dy));
        }

        private static Rect ToRect(PointShape p1, PointShape p2, double dx, double dy)
        {
            double x = Math.Min(p1.X + dx, p2.X + dx);
            double y = Math.Min(p1.Y + dy, p2.Y + dy);
            double width = Math.Abs(Math.Max(p1.X + dx, p2.X + dx) - x);
            double height = Math.Abs(Math.Max(p1.Y + dy, p2.Y + dy) - y);
            return new Rect(x, y, width, height);
        }

        private AvaloniaBrushCache? GetBrushCache(ShapeStyle style)
        {
            if (style == null)
            {
                return null;
            }
            if (!_brushCache.TryGetValue(style, out var cache))
            {
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
                return _brushCache[style];
            }
            return cache;
        }

        private static Matrix ToMatrixTransform(MatrixObject m)
        {
            return new Matrix(m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY);
        }

        private Matrix? GetMatrixCache(MatrixObject matrix)
        {
            if (matrix == null)
            {
                return null;
            }
            if (!_matrixCache.TryGetValue(matrix, out var cache))
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
                return _matrixCache[matrix];
            }
            return cache;
        }

        private static Geometry ToGeometry(CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), false);
                context.CubicBezierTo(
                    ToPoint(cubicBezier.Point1, dx, dy),
                    ToPoint(cubicBezier.Point2, dx, dy),
                    ToPoint(cubicBezier.Point3, dx, dy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), false);
                context.QuadraticBezierTo(
                    ToPoint(quadraticBezier.Point1, dx, dy),
                    ToPoint(quadraticBezier.Point2, dx, dy));
                context.EndFigure(false);
            }

            return geometry;
        }

        private static Geometry ToGeometry(PathShape path, ShapeStyle style, double dx, double dy)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.SetFillRule(path.FillRule == PathFillRule.EvenOdd ? FillRule.EvenOdd : FillRule.NonZero);

                foreach (var figure in path.Figures)
                {
                    bool isFirstShape = true;
                    foreach (var shape in figure.Shapes)
                    {
                        if (shape is LineShape line)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(line.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.LineTo(ToPoint(line.Point, dx, dy));
                        }
                        else if (shape is CubicBezierShape cubicBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(cubicBezier.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.CubicBezierTo(
                                ToPoint(cubicBezier.Point1, dx, dy),
                                ToPoint(cubicBezier.Point2, dx, dy),
                                ToPoint(cubicBezier.Point3, dx, dy));
                        }
                        else if (shape is QuadraticBezierShape quadraticBezier)
                        {
                            if (isFirstShape)
                            {
                                context.BeginFigure(ToPoint(quadraticBezier.StartPoint, dx, dy), figure.IsFilled);
                                isFirstShape = false;
                            }
                            context.QuadraticBezierTo(
                                ToPoint(quadraticBezier.Point1, dx, dy),
                                ToPoint(quadraticBezier.Point2, dx, dy));
                        }
                    }

                    if (!isFirstShape)
                    {
                        context.EndFigure(figure.IsClosed);
                    }
                }
            }

            return geometry;
        }

        private static Geometry ToGeometry(EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var rect = ToRect(ellipse.TopLeft, ellipse.BottomRight, dx, dy);
            return new EllipseGeometry(rect);
        }

        private Geometry GetGeometryCache(CubicBezierShape cubic, ShapeStyle style, double dx, double dy)
        {
            if (cubic == null)
            {
                return null;
            }
            if (!_cubicGeometryCache.TryGetValue(cubic, out var cache))
            {
                var geometry = ToGeometry(cubic, style, dx, dy);
                if (geometry != null)
                {
                    _cubicGeometryCache[cubic] = geometry;
                    return _cubicGeometryCache[cubic];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(QuadraticBezierShape quad, ShapeStyle style, double dx, double dy)
        {
            if (quad == null)
            {
                return null;
            }
            if (!_quadGeometryCache.TryGetValue(quad, out var cache))
            {
                var geometry = ToGeometry(quad, style, dx, dy);
                if (geometry != null)
                {
                    _quadGeometryCache[quad] = geometry;
                    return _quadGeometryCache[quad];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(PathShape path, ShapeStyle style, double dx, double dy)
        {
            if (path == null)
            {
                return null;
            }
            if (!_pathGeometryCache.TryGetValue(path, out var cache))
            {
                var geometry = ToGeometry(path, style, dx, dy);
                if (geometry != null)
                {
                    _pathGeometryCache[path] = geometry;
                    return _pathGeometryCache[path];
                }
                return null;
            }
            return cache;
        }

        private Geometry GetGeometryCache(EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            if (ellipse == null)
            {
                return null;
            }
            if (!_ellipseGeometryCache.TryGetValue(ellipse, out var cache))
            {
                var geometry = ToGeometry(ellipse, style, dx, dy);
                if (geometry != null)
                {
                    _ellipseGeometryCache[ellipse] = geometry;
                    return _ellipseGeometryCache[ellipse];
                }
                return null;
            }
            return cache;
        }

        private FormattedTextCache GetTextCache(TextShape text, Rect rect)
        {
            if (!_formattedTextCache.TryGetValue(text, out var cache))
            {
                _formattedTextCache[text] = FormattedTextCache.FromTextShape(text, rect);
                return _formattedTextCache[text];
            }
            return cache;
        }

        public void InvalidateCache(ShapeStyle style)
        {
            if (style != null)
            {
                if (!_brushCache.TryGetValue(style, out var cache))
                {
                    cache.Dispose();
                }
                _brushCache[style] = AvaloniaBrushCache.FromDrawStyle(style);
            }
        }

        public void InvalidateCache(MatrixObject matrix)
        {
            if (matrix != null)
            {
                _matrixCache[matrix] = ToMatrixTransform(matrix);
            }
        }

        public void InvalidateCache(BaseShape shape, ShapeStyle style, double dx, double dy)
        {
            switch (shape)
            {
                case CubicBezierShape cubic:
                    {
                        var geometry = ToGeometry(cubic, style, dx, dy);
                        if (geometry != null)
                        {
                            _cubicGeometryCache[cubic] = geometry;
                        }
                    }
                    break;
                case QuadraticBezierShape quad:
                    {
                        var geometry = ToGeometry(quad, style, dx, dy);
                        if (geometry != null)
                        {
                            _quadGeometryCache[quad] = geometry;
                        }
                    }
                    break;
                case PathShape path:
                    {
                        var geometry = ToGeometry(path, style, dx, dy);
                        if (geometry != null)
                        {
                            _pathGeometryCache[path] = geometry;
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        var geometry = ToGeometry(ellipse, style, dx, dy);
                        if (geometry != null)
                        {
                            _ellipseGeometryCache[ellipse] = geometry;
                        }
                    }
                    break;
                case TextShape text:
                    {
                        if (!_formattedTextCache.TryGetValue(text, out var cache))
                        {
                            cache.Dispose();
                        }
                        var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);
                        _formattedTextCache[text] = FormattedTextCache.FromTextShape(text, rect);
                    }
                    break;
            }
        }

        public object PushMatrix(object dc, MatrixObject matrix)
        {
            var _dc = dc as DrawingContext;
            return _dc.PushPreTransform(GetMatrixCache(matrix).Value);
        }

        public void PopMatrix(object dc, object state)
        {
            var _state = (DrawingContext.PushedState)state;
            _state.Dispose();
        }

        public void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            _dc.DrawLine(style.IsStroked ? cache?.StrokePen : null, ToPoint(line.StartPoint, dx, dy), ToPoint(line.Point, dx, dy));
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(cubicBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(quadraticBezier, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(path, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(rectangle.TopLeft, rectangle.BottomRight, dx, dy);
            if (style.IsFilled)
            {
                _dc.FillRectangle(cache?.Fill, rect);
            }
            if (style.IsStroked)
            {
                _dc.DrawRectangle(cache?.StrokePen, rect);
            }
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var geometry = GetGeometryCache(ellipse, style, dx, dy);
            _dc.DrawGeometry(style.IsFilled ? cache?.Fill : null, style.IsStroked ? cache?.StrokePen : null, geometry);
        }

        public void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy)
        {
            var cache = GetBrushCache(style);
            var _dc = dc as DrawingContext;
            var rect = ToRect(text.TopLeft, text.BottomRight, dx, dy);

            //if (style.IsFilled)
            //{
            //    _dc.FillRectangle(cache?.Fill, rect);
            //}

            //if (style.IsStroked)
            //{
            //    _dc.DrawRectangle(cache?.StrokePen, rect);
            //}

            if (text.Text != null)
            {
                var ftc = GetTextCache(text, rect);
                _dc.DrawText(cache?.Stroke, ftc.Origin, ftc.FormattedText);
            }
        }
    }

    public class InputView : Border
    {
        public static readonly StyledProperty<IVisual> RelativeToProperty =
            AvaloniaProperty.Register<InputView, IVisual>(nameof(RelativeTo));

        public IVisual RelativeTo
        {
            get { return GetValue(RelativeToProperty); }
            set { SetValue(RelativeToProperty, value); }
        }

        public InputView()
        {
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        private Modifier GetModifier(InputModifiers inputModifiers)
        {
            Modifier modifier = Modifier.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= Modifier.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= Modifier.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= Modifier.Shift;
            }

            return modifier;
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is MainViewModel vm)
                {
                    var point = e.GetPosition(RelativeTo);
                    vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is MainViewModel vm)
                {
                    var point = e.GetPosition(RelativeTo);
                    vm.CurrentTool.RightDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is MainViewModel vm)
                {
                    var point = e.GetPosition(RelativeTo);
                    if (vm.Mode == EditMode.Mouse)
                    {
                        vm.CurrentTool.LeftUp(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    }
                    else if (vm.Mode == EditMode.Touch)
                    {
                        vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    }
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is MainViewModel vm)
                {
                    var point = e.GetPosition(RelativeTo);
                    vm.CurrentTool.RightUp(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (this.DataContext is MainViewModel vm)
            {
                var point = e.GetPosition(RelativeTo);
                vm.CurrentTool.Move(vm, point.X, point.Y, GetModifier(e.InputModifiers));
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is MainViewModel vm)
            {
                if (vm.CurrentContainer.WorkBackground != null)
                {
                    var color = AvaloniaBrushCache.FromDrawColor(vm.CurrentContainer.InputBackground);
                    var brush = new SolidColorBrush(color);
                    context.FillRectangle(brush, new Rect(0, 0, Bounds.Width, Bounds.Height));
                }
            }
        }
    }

    public class RenderView : Canvas
    {
        private bool _drawWorking = false;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.DataContext is MainViewModel vm)
            {
                var md = (this.GetVisualRoot() as IInputRoot)?.MouseDevice;
                if (md != null)
                {
                    vm.Capture = () =>
                    {
                        if (md.Captured == null)
                        {
                            md.Capture(this);
                        }
                    };
                    vm.Release = () =>
                    {
                        if (md.Captured != null)
                        {
                            md.Capture(null);
                        }
                    };
                    vm.Invalidate = () => this.InvalidateVisual();
                }
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (this.DataContext is MainViewModel vm)
            {
                vm.Capture = null;
                vm.Release = null;
                vm.Invalidate = null;
            }
        }

        protected override void OnPointerEnter(PointerEventArgs e)
        {
            base.OnPointerEnter(e);
            _drawWorking = true;
            this.InvalidateVisual();
        }

        protected override void OnPointerLeave(PointerEventArgs e)
        {
            base.OnPointerLeave(e);
            _drawWorking = false;
            this.InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (this.DataContext is MainViewModel vm)
            {
                if (vm.CurrentContainer.WorkBackground != null)
                {
                    var color = AvaloniaBrushCache.FromDrawColor(vm.CurrentContainer.WorkBackground);
                    var brush = new SolidColorBrush(color);
                    context.FillRectangle(brush, new Rect(0, 0, Bounds.Width, Bounds.Height));
                }

                vm.Presenter.DrawContainer(context, vm.CurrentContainer, vm.Renderer, 0.0, 0.0, null, null);

                if (_drawWorking)
                {
                    vm.Presenter.DrawContainer(context, vm.WorkingContainer, vm.Renderer, 0.0, 0.0, null, null);
                }

                vm.Presenter.DrawDecorators(context, vm.CurrentContainer, vm.Renderer, 0.0, 0.0);

                if (_drawWorking)
                {
                    vm.Presenter.DrawDecorators(context, vm.WorkingContainer, vm.Renderer, 0.0, 0.0);
                }
            }
        }
    }
}
