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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Draw2D.Editor
{
    internal class JsonSerializer
    {
        internal class CoreContractResolver : DefaultContractResolver
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

        static JsonSerializer()
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

    public interface IContainerEditor : IToolContext
    {
        T Load<T>(string path);
        void Save<T>(string path, T value);
        void New();
        void Open();
        void SaveAs();
        void OpenContainer(string path);
        void SaveContainer(string path);
        void Exit();
    }

    public class CanvasPresenter : ICanvasPresenter
    {
        public IDictionary<Type, IShapeDecorator> Decorators { get; set; }

        public void DrawContainer(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            container.Invalidate(renderer, dx, dy);
            container.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public void DrawDecorators(object dc, CanvasContainer container, IShapeRenderer renderer, double dx, double dy, DrawMode mode)
        {
            var shapes = container.Shapes;
            var selection = renderer.Selection;

            foreach (var shape in shapes)
            {
                if (selection.Selected.Contains(shape))
                {
                    if (Decorators.TryGetValue(shape.GetType(), out var helper))
                    {
                        helper.Draw(dc, shape, renderer, selection, dx, dy, mode);
                    }
                }
            }
        }
    }

    public class ContainerEditor : ToolContext, IContainerEditor
    {
        public static void CreateDemoGroup(IToolContext context)
        {
            var container = context.ContainerView.CurrentContainer;
            var group = new GroupShape();
            group.Shapes.Add(
                new RectangleShape(new PointShape(30, 30, container.PointTemplate), 
                new PointShape(60, 60, container.PointTemplate))
            {
                Style = container.CurrentStyle
            });
            group.Points.Add(new PointShape(45, 30, container.PointTemplate));
            group.Points.Add(new PointShape(45, 60, container.PointTemplate));
            group.Points.Add(new PointShape(30, 45, container.PointTemplate));
            group.Points.Add(new PointShape(60, 45, container.PointTemplate));
            context.ContainerView.CurrentContainer.Shapes.Add(group);
        }

        public ContainerEditor()
        {
            Initialize();
            NewContainer();
        }

        private void Initialize()
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
                Filters = new ObservableCollection<PointFilter>
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
                Filters = new ObservableCollection<PointFilter>
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

            var containerView = new ContainerView()
            {
                InputService = null,
                Renderer = renderer,
                Presenter = presenter,
                Selection = selectionTool,
                CurrentContainer = null,
                WorkingContainer = null
            };

            ContainerView = containerView;
            Tools = tools;
            CurrentTool = currentTool;
            Mode = EditMode.Mouse;
            HitTest = hitTest;
        }

        private void NewContainer()
        {
            ContainerView.CurrentContainer = new CanvasContainer()
            {
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 128, 128, 128),
                InputBackground = new ArgbColor(255, 211, 211, 211),
                CurrentStyle = new ShapeStyle(new ArgbColor(255, 0, 255, 0), new ArgbColor(80, 0, 255, 0), 2.0, true, true),
                PointTemplate = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
                {
                    Style = new ShapeStyle(new ArgbColor(0, 0, 0, 0), new ArgbColor(255, 255, 255, 0), 2.0, true, true)
                }
            };
            ContainerView.WorkingContainer = new CanvasContainer();
        }

        public T Load<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.FromJson<T>(json);
        }

        public void Save<T>(string path, T value)
        {
            var json = JsonSerializer.ToJson<T>(value);
            File.WriteAllText(path, json);
        }

        public void New()
        {
            CurrentTool.Clean(this);
            ContainerView.Selection.Selected.Clear();
            NewContainer();
            ContainerView.InputService?.Redraw?.Invoke();
        }

        public void OpenContainer(string path)
        {
            var container = Load<CanvasContainer>(path);
            var workingContainer = new CanvasContainer();
            CurrentTool.Clean(this);
            ContainerView.Selection.Selected.Clear();
            ContainerView.CurrentContainer = container;
            ContainerView.WorkingContainer = workingContainer;
            ContainerView.InputService?.Redraw?.Invoke();
        }

        public void SaveContainer(string path)
        {
            Save(path, ContainerView.CurrentContainer);
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
                OpenContainer(path);
            }
        }

        public async void SaveAs()
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
                SaveContainer(path);
            }
        }

        public void Exit()
        {
            Application.Current.Windows.FirstOrDefault()?.Close();
        }
    }
}
