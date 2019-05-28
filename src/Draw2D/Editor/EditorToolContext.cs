// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//#define USE_SVG_POINT
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Draw2D.Editor.Renderers;
using Draw2D.Editor.Views;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using SkiaSharp;

namespace Draw2D.Editor
{
    [DataContract(IsReference = true)]
    public class EditorToolContext : ToolContext
    {
        private IContainerFactory _containerFactory;
        private ISelection _selection;
        private string _currentDirectory;
        private IList<string> _files;

        [IgnoreDataMember]
        public IContainerFactory ContainerFactory
        {
            get => _containerFactory;
            set => Update(ref _containerFactory, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISelection Selection
        {
            get => _selection;
            set => Update(ref _selection, value);
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

        public EditorToolContext()
        {
        }

        public EditorToolContext(IContainerFactory containerFactory)
        {
            _containerFactory = containerFactory;
        }

        private ShapeStyle GetShapeStyle(string styleId)
        {
            if (StyleLibrary?.Styles != null)
            {
                foreach (var style in StyleLibrary.Styles)
                {
                    if (style.Title == styleId)
                    {
                        return style;
                    }
                }
            }
            return null;
        }

        public void InitContainerView(IContainerView containerView)
        {
            containerView.DrawContainerView = new AvaloniaSkiaView(this);

            containerView.WorkingContainer = new CanvasContainer()
            {
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>()
            };
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
                containerView.WorkingContainer = null;
            }
        }

        public void NewContainerView(string title)
        {
            var containerView = _containerFactory?.CreateContainerView(title);
            if (containerView != null)
            {
                InitContainerView(containerView);
                AddContainerView(containerView);
            }
        }

        public void OpenView(string path)
        {
            var containerView = JsonSerializer.FromJsonFile<ContainerView>(path);
            if (containerView != null)
            {
                InitContainerView(containerView);
                AddContainerView(containerView);
            }
        }

        public void SaveView(string path)
        {
            if (ContainerView != null)
            {
                JsonSerializer.ToJsonFile(path, ContainerView);
            }
        }

        public void OpenStyles(string path)
        {
            var styleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(path);
            if (styleLibrary != null)
            {
                StyleLibrary = styleLibrary;
                StyleLibrary.UpdateCache();
            }
        }

        public void SaveStyles(string path)
        {
            if (StyleLibrary != null)
            {
                JsonSerializer.ToJsonFile(path, StyleLibrary);
            }
        }

        public void OpenGroups(string path)
        {
            var groupLibrary = JsonSerializer.FromJsonFile<IGroupLibrary>(path);
            if (groupLibrary != null)
            {
                GroupLibrary = groupLibrary;
                GroupLibrary.UpdateCache();
            }
        }

        public void SaveGroups(string path)
        {
            if (GroupLibrary != null)
            {
                JsonSerializer.ToJsonFile(path, GroupLibrary);
            }
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
                    OpenView(path);
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
                SaveView(path);
            }
        }

        public async void OpenStyleLibrary()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.AllowMultiple = false;
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                foreach (var path in result)
                {
                    OpenStyles(path);
                }
            }
        }

        public async void SaveStyleLibraryAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = "styles";
            dlg.DefaultExtension = "json";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                SaveStyles(path);
            }
        }

        public async void OpenGroupLibrary()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.AllowMultiple = false;
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                foreach (var path in result)
                {
                    OpenGroups(path);
                }
            }
        }

        public async void SaveGroupLibraryAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = "groups";
            dlg.DefaultExtension = "json";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                SaveGroups(path);
            }
        }

        public void AddFiles(string path)
        {
            foreach (var file in Directory.EnumerateFiles(path, "*.json"))
            {
                Files.Add(file);
            }
        }

        public void ClearFiles()
        {
            Files.Clear();
        }

        public void ImportSvg(string path)
        {
            var svg = new SkiaSharp.Extended.Svg.SKSvg();
            var picture = svg.Load(path);
            // TODO: Convert picture to shapes.
        }

        public static void ExportSvg(string path, IContainerView containerView, IToolContext context)
        {
            using (var stream = new SKFileWStream(path))
            using (var writer = new SKXmlStreamWriter(stream))
            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)containerView.Width, (int)containerView.Height), writer))
            using (var skiaView = new ExportSkiaView(context))
            {
                skiaView.Draw(containerView, canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
            }
        }

        public static void ExportPng(string path, IContainerView containerView, IToolContext context)
        {
            var info = new SKImageInfo((int)containerView.Width, (int)containerView.Height);
            using (var bitmap = new SKBitmap(info))
            {
                using (var canvas = new SKCanvas(bitmap))
                using (var skiaView = new ExportSkiaView(context))
                {
                    skiaView.Draw(containerView, canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                }
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(path))
                {
                    data.SaveTo(stream);
                }
            }
        }

        public static void ExportPdf(string path, IContainerView containerView, IToolContext context)
        {
            using (var stream = new SKFileWStream(path))
            using (var pdf = SKDocument.CreatePdf(stream, 72.0f))
            using (var canvas = pdf.BeginPage((float)containerView.Width, (float)containerView.Height))
            using (var skiaView = new ExportSkiaView(context))
            {
                skiaView.Draw(containerView, canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                pdf.Close();
            }
        }

        public static void Export(string path, IContainerView containerView, IToolContext context)
        {
            var outputExtension = Path.GetExtension(path);

            if (string.Compare(outputExtension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportSvg(path, containerView, context);
            }
            else if (string.Compare(outputExtension, ".png", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportPng(path, containerView, context);
            }
            else if (string.Compare(outputExtension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportPdf(path, containerView, context);
            }
        }

        public async void ImportFile()
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

        public async void ExportFile()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf Files", Extensions = { "pdf" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg Files", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Png Files", Extensions = { "png" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = ContainerView.Title;
            dlg.DefaultExtension = "pdf";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                Export(path, ContainerView, this);
            }
        }

        public void FromSvgPathData(TextBox textBox)
        {
            var svgPathData = textBox.Text;
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                var path = SkiaHelper.ToGeometry(svgPathData);
                var pathShape = SkiaHelper.FromGeometry(path, StyleLibrary.CurrentStyle, PointTemplate);
                ContainerView.CurrentContainer.Shapes.Add(pathShape);
                ContainerView.CurrentContainer.MarkAsDirty(true);
                ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        private void ToSvgPathDataImpl(IBaseShape shape, StringBuilder sb)
        {
            switch (shape)
            {
                case LineShape line:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(line, 0.0, 0.0).ToSvgPathData());
                        // TODO: Convert Text on path to geometry.
                    }
                    break;
                case CubicBezierShape cubicBezier:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(cubicBezier, 0.0, 0.0).ToSvgPathData());
                        // TODO: Convert Text on path to geometry.
                    }
                    break;
                case QuadraticBezierShape quadraticBezier:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(quadraticBezier, 0.0, 0.0).ToSvgPathData());
                        // TODO: Convert Text on path to geometry.
                    }
                    break;
                case ConicShape conic:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(conic, 0.0, 0.0).ToSvgPathData());
                        // TODO: Convert Text on path to geometry.
                    }
                    break;
                case PathShape pathShape:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(pathShape, 0.0, 0.0).ToSvgPathData());
                        // TODO: Convert Text on path to geometry.
                    }
                    break;
                case RectangleShape rectangle:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(rectangle, 0.0, 0.0).ToSvgPathData());
                        if (!string.IsNullOrEmpty(rectangle.Text?.Value))
                        {
                            var style = GetShapeStyle(rectangle.StyleId);
                            if (style != null)
                            {
                                SkiaHelper.ToGeometry(rectangle.Text, rectangle.TopLeft, rectangle.BottomRight, style.TextStyle, 0.0, 0.0);
                            }
                        }
                    }
                    break;
                case EllipseShape ellipse:
                    {
                        sb.AppendLine(SkiaHelper.ToGeometry(ellipse, 0.0, 0.0).ToSvgPathData());
                        if (!string.IsNullOrEmpty(ellipse.Text?.Value))
                        {
                            var style = GetShapeStyle(ellipse.StyleId);
                            if (style != null)
                            {
                                SkiaHelper.ToGeometry(ellipse.Text, ellipse.TopLeft, ellipse.BottomRight, style.TextStyle, 0.0, 0.0);
                            }
                        }
                    }
                    break;
#if USE_SVG_POINT
                case IPointShape point:
                    {
                        if (point.Template != null)
                        {
                            ToSvgPathDataImpl(point.Template, sb);
                        }
                    }
                    break;
#endif
                case GroupShape group:
                    {
                        foreach (var groupShape in group.Shapes)
                        {
                            ToSvgPathDataImpl(groupShape, sb);
                        }
                    }
                    break;
                case TextShape text:
                    {
                        var style = GetShapeStyle(text.StyleId);
                        if (style != null)
                        {
                            sb.AppendLine(SkiaHelper.ToGeometry(text, style.TextStyle, 0.0, 0.0).ToSvgPathData());
                        }
                    }
                    break;
            };
        }

        public void ToSvgPathData(TextBox textBox)
        {
            if (ContainerView.SelectionState?.Shapes != null)
            {
                var selected = new List<IBaseShape>(ContainerView.SelectionState?.Shapes);
                var sb = new StringBuilder();

                foreach (var shape in selected)
                {
                    ToSvgPathDataImpl(shape, sb);
                }

                var svgPathData = sb.ToString();
                textBox.Text = svgPathData;
                Application.Current.Clipboard.SetTextAsync(svgPathData);
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
                Points = new ObservableCollection<IPointShape>(),
                Shapes = new ObservableCollection<IBaseShape>()
            };

            var rectangle = new RectangleShape(new PointShape(0, 0, context.PointTemplate), new PointShape(30, 30, context.PointTemplate))
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = new Text(),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            rectangle.TopLeft.Owner = rectangle;
            rectangle.BottomRight.Owner = rectangle;

            var text = new TextShape(new PointShape(0, 0, context.PointTemplate), new PointShape(30, 30, context.PointTemplate))
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = new Text("&"),
                StyleId = context.StyleLibrary?.CurrentStyle?.Title
            };
            text.TopLeft.Owner = text;
            text.BottomRight.Owner = text;

            group.Shapes.Add(rectangle);
            group.Shapes.Add(text);

            group.Points.Add(new PointShape(15, 0, context.PointTemplate));
            group.Points.Add(new PointShape(15, 30, context.PointTemplate));
            group.Points.Add(new PointShape(0, 15, context.PointTemplate));
            group.Points.Add(new PointShape(30, 15, context.PointTemplate));

            foreach (var point in group.Points)
            {
                point.Owner = group;
            }

            var reference1 = new ReferenceShape(group.Title, 0, 60, group);
            var reference2 = new ReferenceShape(group.Title, 0, 120, group);

            context.ContainerView?.CurrentContainer.Shapes.Add(group);
            context.ContainerView?.CurrentContainer.Shapes.Add(reference1);
            context.ContainerView?.CurrentContainer.Shapes.Add(reference2);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
        }
    }
}
