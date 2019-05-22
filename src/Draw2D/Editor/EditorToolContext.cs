// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Avalonia;
using Avalonia.Controls;
using Draw2D.Editor.Renderers;
using Draw2D.Editor.Views;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using SkiaSharp;

namespace Draw2D.Editor
{
    [DataContract(IsReference = true)]
    public class EditorToolContext : ToolContext
    {
        private IFactory _factory;
        private ISelection _selection;
        private string _currentDirectory;
        private IList<string> _files;

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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IFactory Factory
        {
            get => _factory;
            set => Update(ref _factory, value);
        }

        public EditorToolContext()
        {
        }

        public EditorToolContext(IFactory factory)
        {
            _factory = factory;
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

        public void InitContainerView(IContainerView containerView)
        {
            containerView.DrawContainerView = new AvaloniaSkiaView();

            containerView.WorkingContainer = new CanvasContainer()
            {
                Shapes = new ObservableCollection<BaseShape>()
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
            var containerView = _factory?.CreateContainerView(title);
            if (containerView != null)
            {
                InitContainerView(containerView);
                AddContainerView(containerView);
            }
        }

        public void Open(string path)
        {
            var containerView = LoadFromJson<ContainerView>(path);
            if (containerView != null)
            {
                InitContainerView(containerView);
                AddContainerView(containerView);
            }
        }

        public void Save(string path)
        {
            if (ContainerView != null)
            {
                SaveAsjson(path, ContainerView);
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

        public static void ExportSvg(string path, IContainerView containerView)
        {
            using (var stream = new SKFileWStream(path))
            using (var writer = new SKXmlStreamWriter(stream))
            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)containerView.Width, (int)containerView.Height), writer))
            {
                var skiaView = new ExportSkiaView();
                skiaView.Draw(containerView, canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
            }
        }

        public static void ExportPng(string path, IContainerView containerView)
        {
            var info = new SKImageInfo((int)containerView.Width, (int)containerView.Height);
            using (var bitmap = new SKBitmap(info))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    var skiaView = new ExportSkiaView();
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

        public static void ExportPdf(string path, IContainerView containerView)
        {
            using (var stream = new SKFileWStream(path))
            using (var pdf = SKDocument.CreatePdf(stream, 72.0f))
            using (var canvas = pdf.BeginPage((float)containerView.Width, (float)containerView.Height))
            {
                var skiaView = new ExportSkiaView();
                skiaView.Draw(containerView, canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                pdf.Close();
            }
        }

        public static void Export(string path, IContainerView containerView)
        {
            var outputExtension = Path.GetExtension(path);

            if (string.Compare(outputExtension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportSvg(path, containerView);
            }
            else if (string.Compare(outputExtension, ".png", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportPng(path, containerView);
            }
            else if (string.Compare(outputExtension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportPdf(path, containerView);
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
                Export(path, ContainerView);
            }
        }

        public void FromSvgPathData(TextBox textBox)
        {
            var svgPathData = textBox.Text;
            if (!string.IsNullOrWhiteSpace(svgPathData))
            {
                var path = SkiaHelper.ToGeometry(svgPathData);
                var pathShape = SkiaHelper.FromGeometry(path, CurrentStyle, PointTemplate);
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
                new RectangleShape(
                    new PointShape(30, 30, context.PointTemplate),
                    new PointShape(60, 60, context.PointTemplate))
                {
                    Points = new ObservableCollection<PointShape>(),
                    Style = context.CurrentStyle
                });
            group.Points.Add(new PointShape(45, 30, context.PointTemplate));
            group.Points.Add(new PointShape(45, 60, context.PointTemplate));
            group.Points.Add(new PointShape(30, 45, context.PointTemplate));
            group.Points.Add(new PointShape(60, 45, context.PointTemplate));
            context.ContainerView?.CurrentContainer.Shapes.Add(group);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
        }
    }
}
