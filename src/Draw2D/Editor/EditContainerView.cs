// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Avalonia;
using Avalonia.Controls;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Filters;
using Draw2D.ViewModels.Intersections;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;
using SkiaSharp.Extended.Svg;

namespace Draw2D.Editor
{
    [DataContract(IsReference = true)]
    public class EditContainerView : ToolContext
    {
        private ISelection _selection;
        private IHitTest _hitTest;
        private string _currentDirectory;
        private IList<string> _files;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISelection Selection
        {
            get => _selection;
            set => Update(ref _selection, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IHitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => Update(ref _currentDirectory, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<string> Files
        {
            get => _files;
            set => Update(ref _files, value);
        }

        public EditContainerView()
        {
        }

        public static T LoadFromJson<T>(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.FromJson<T>(json);
        }

        public static void SaveAsjson<T>(string path, T value)
        {
            var json = JsonSerializer.ToJson<T>(value);
            File.WriteAllText(path, json);
        }

        public void Initialize()
        {
            var hitTest = new HitTest();

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
                        "GuideStyle",
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
                        "GuideStyle",
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
                                "GuideStyle",
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
                        "SelectionStyle",
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

            Selection = selectionTool;
            HitTest = hitTest;
            CurrentDirectory = null;
            Files = new ObservableCollection<string>();

            var containerViews = new ObservableCollection<IContainerView>();

            var containerView0 = CreateContainerView("View0");
            InitContainerView(containerView0);

            var containerView1 = CreateContainerView("View1");
            InitContainerView(containerView1);

            var containerView2 = CreateContainerView("View2");
            InitContainerView(containerView2);

            containerViews.Add(containerView0);
            containerViews.Add(containerView1);
            containerViews.Add(containerView2);

            ContainerViews = containerViews;
            ContainerView = containerViews.FirstOrDefault();
            Tools = tools;
            CurrentTool = currentTool;
            Mode = EditMode.Mouse;
        }

        public IContainerView CreateContainerView(string title)
        {
            var containerView = new ContainerView()
            {
                Title = title,
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(255, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 128, 128, 128),
                InputBackground = new ArgbColor(255, 211, 211, 211),
                Styles = new ObservableCollection<ShapeStyle>(),
                CurrentStyle = new ShapeStyle(
                    "CurrentStyle",
                    new ArgbColor(255, 0, 255, 0),
                    new ArgbColor(80, 0, 255, 0),
                    2.0, true, true,
                    new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 0), true)),
                PointTemplate = new EllipseShape(new PointShape(-3, -3, null), new PointShape(3, 3, null))
                {
                    Points = new ObservableCollection<PointShape>(),
                    Style = new ShapeStyle(
                        "Style",
                        new ArgbColor(255, 255, 255, 0),
                        new ArgbColor(255, 255, 255, 0),
                        2.0, true, true,
                        new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 255, 0), true))
                },
                InputService = null,
                DrawContainerView = null,
                SelectionState = new SelectionState()
                {
                    Hovered = null,
                    Selected = null,
                    Shapes = new HashSet<BaseShape>()
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
                CurrentContainer = new CanvasContainer()
                {
                    Shapes = new ObservableCollection<BaseShape>()
                },
                WorkingContainer = null,
                HitTest = null
            };

            containerView.Styles.Add(containerView.CurrentStyle);

            return containerView;
        }

        public void InitContainerView(IContainerView containerView)
        {
            if (containerView != null)
            {
                containerView.DrawContainerView = new AvaloniaSkiaView();
                containerView.HitTest = _hitTest;
                containerView.WorkingContainer = new CanvasContainer()
                {
                    Shapes = new ObservableCollection<BaseShape>()
                };
            }
        }

        public void AddContainerView(IContainerView containerView)
        {
            if (containerView != null)
            {
                CurrentTool.Clean(this);
                ContainerView?.SelectionState.Clear();

                ContainerViews.Add(containerView);
                ContainerView = containerView;

                ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void CloseContainerView(IContainerView containerView)
        {
            if (containerView != null)
            {
                int index = ContainerViews.IndexOf(containerView);
                if (index >= 0)
                {
                    ContainerViews.Remove(containerView);

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

                    containerView.DrawContainerView.Dispose();
                }
                containerView.DrawContainerView = null;
                containerView.SelectionState = null;
                containerView.HitTest = null;
                containerView.WorkingContainer = null;
            }
        }

        public void NewContainerView()
        {
            var containerView = CreateContainerView("View");
            InitContainerView(containerView);
            AddContainerView(containerView);
        }

        public void Open(string path)
        {
            var containerView = LoadFromJson<ContainerView>(path);
            InitContainerView(containerView);
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
            dlg.AllowMultiple = true;
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                foreach (var path in result)
                {
                    Open(path);
                }
            }
        }

        public async void SaveContainerViewAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = ContainerView.Title;
            dlg.DefaultExtension = "json";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                Save(path);
            }
        }

        public void AddFiles(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.json"))
            {
                Files.Add(file);
            }
        }

        public void ClearFiles(string path)
        {
            Files.Clear();
        }

        public void ImportSvg(string path)
        {
            var svg = new SkiaSharp.Extended.Svg.SKSvg();
            var picture = svg.Load(path);
            // TODO: Convert picture to shapes.
        }

        private void Export(SKCanvas canvas, IContainerView view)
        {
            var presenter = new ExportSkiaPresenter();
            presenter.Draw(view, canvas, view.Width, view.Height, 0, 0, 1.0, 1.0);
        }

        public void ExportSvg(string path, IContainerView view)
        {
            using (var stream = new SKFileWStream(path))
            using (var writer = new SKXmlStreamWriter(stream))
            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)view.Width, (int)view.Height), writer))
            {
                Export(canvas, view);
            }
        }

        public void ExportPng(string path, IContainerView view)
        {
            var info = new SKImageInfo((int)view.Width, (int)view.Height);
            using (var bitmap = new SKBitmap(info))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    Export(canvas, view);
                }
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(path))
                {
                    data.SaveTo(stream);
                }
            }
        }

        public void ExportPdf(string path, IContainerView view)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream, 72.0f))
                {
                    using (var canvas = pdf.BeginPage((float)view.Width, (float)view.Height))
                    {
                        Export(canvas, view);
                    }
                    pdf.Close();
                }
            }
        }

        public async void ImportSvgFile()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg Files", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.AllowMultiple = true;
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                foreach (var path in result)
                {
                    ImportSvg(path);
                }
            }
        }

        public async void ExportSvgFile()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg Files", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = ContainerView.Title;
            dlg.DefaultExtension = "svg";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                ExportSvg(path, ContainerView);
            }
        }

        public async void ExportPngFile()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Png Files", Extensions = { "png" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = ContainerView.Title;
            dlg.DefaultExtension = "png";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                ExportPng(path, ContainerView);
            }
        }

        public async void ExportPdfFile()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf Files", Extensions = { "pdf" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = ContainerView.Title;
            dlg.DefaultExtension = "pdf";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                ExportPdf(path, ContainerView);
            }
        }

        public void FromSvgPathData(TextBox textBox)
        {
            var svgPathData = textBox.Text;
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                var path = SkiaHelper.ToGeometry(svgPathData);
                var pathShape = SkiaHelper.FromGeometry(path, ContainerView.CurrentStyle, ContainerView.PointTemplate);
                ContainerView.CurrentContainer.Shapes.Add(pathShape);
                ContainerView.CurrentContainer.MarkAsDirty(true);
                ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void ToSvgPathData(TextBox textBox)
        {
            var selected = ContainerView.SelectionState.Shapes;
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
                            var path = SkiaHelper.ToGeometry(lineShape, 0.0, 0.0);
                            var svgPathData = path.ToSvgPathData();
                            textBox.Text = svgPathData;
                            Application.Current.Clipboard.SetTextAsync(svgPathData);
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
            Application.Current.Shutdown();
        }

        public void CreateDemoGroup(IToolContext context)
        {
            var group = new GroupShape()
            {
                Title = "Group",
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
    }
}
