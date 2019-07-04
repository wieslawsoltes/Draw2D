// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Draw2D.Presenters;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Editor
{
    [DataContract(IsReference = true)]
    public class EditorToolContext : ToolContext, IEditorToolContext
    {
        public new static EditMode[] EditModeValues { get; } = (EditMode[])Enum.GetValues(typeof(EditMode));

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

        public void InitContainerView(IContainerView containerView)
        {
            containerView.ContainerPresenter = new AvaloniaContainerPresenter(this, containerView);

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

                    containerView.ContainerPresenter.Dispose();
                }
                containerView.ContainerPresenter = null;
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
            var result = await dlg.ShowAsync(GetWindow());
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
            var result = await dlg.ShowAsync(GetWindow());
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
            var result = await dlg.ShowAsync(GetWindow());
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
            var result = await dlg.ShowAsync(GetWindow());
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
            var result = await dlg.ShowAsync(GetWindow());
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
            var result = await dlg.ShowAsync(GetWindow());
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

        public static void ImportSvg(IToolContext context, string path)
        {
            var svg = new SkiaSharp.Extended.Svg.SKSvg();
            using (var stream = File.Open(path, FileMode.Open))
            {
                var picture = svg.Load(stream);
                //var image = SKImage.FromPicture(picture, picture.CullRect.Size.ToSizeI());
            }
        }

        public static void ExportSvg(IToolContext context, string path, IContainerView containerView)
        {
            using (var stream = new SKFileWStream(path))
            using (var writer = new SKXmlStreamWriter(stream))
            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)containerView.Width, (int)containerView.Height), writer))
            using (var skiaContainerPresenter = new ExportContainerPresenter(context, containerView))
            {
                skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
            }
        }

        public static void ExportPdf(IToolContext context, string path, IContainerView containerView)
        {
            using (var stream = new SKFileWStream(path))
            using (var pdf = SKDocument.CreatePdf(stream, SKDocument.DefaultRasterDpi))
            using (var canvas = pdf.BeginPage((float)containerView.Width, (float)containerView.Height))
            using (var skiaContainerPresenter = new ExportContainerPresenter(context, containerView))
            {
                skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                pdf.Close();
            }
        }

        public static void ExportXps(IToolContext context, string path, IContainerView containerView)
        {
            using (var stream = new SKFileWStream(path))
            using (var xps = SKDocument.CreateXps(stream, SKDocument.DefaultRasterDpi))
            using (var canvas = xps.BeginPage((float)containerView.Width, (float)containerView.Height))
            using (var skiaContainerPresenter = new ExportContainerPresenter(context, containerView))
            {
                skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                xps.Close();
            }
        }

        public static void ExportSkp(IToolContext context, string path, IContainerView containerView)
        {
            var recorder = new SKPictureRecorder();
            var rect = new SKRect(0f, 0f, (float)containerView.Width, (float)containerView.Height);
            var canvas = recorder.BeginRecording(rect);
            using (var skiaContainerPresenter = new ExportContainerPresenter(context, containerView))
            {
                skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
            }
            var picture = recorder.EndRecording();
            var dimensions = new SKSizeI((int)containerView.Width, (int)containerView.Height);
            using (var image = SKImage.FromPicture(picture, dimensions))
            {
                var data = image.EncodedData;
                if (data != null)
                {
                    using (var stream = File.OpenWrite(path))
                    {
                        data.SaveTo(stream);
                    }
                }
            }
            picture.Dispose();
        }

        public static void ExportImage(IToolContext context, string path, IContainerView containerView, SKEncodedImageFormat format, int quality)
        {
            var info = new SKImageInfo((int)containerView.Width, (int)containerView.Height);
            using (var bitmap = new SKBitmap(info))
            {
                using (var canvas = new SKCanvas(bitmap))
                using (var skiaContainerPresenter = new ExportContainerPresenter(context, containerView))
                {
                    skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                }
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(format, quality))
                {
                    if (data != null)
                    {
                        using (var stream = File.OpenWrite(path))
                        {
                            data.SaveTo(stream);
                        }
                    }
                }
            }
        }

        public static void Export(IToolContext context, string path, IContainerView containerView)
        {
            var outputExtension = Path.GetExtension(path);

            if (string.Compare(outputExtension, ".svg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportSvg(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".pdf", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportPdf(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".xps", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportXps(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".skp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportSkp(context, path, containerView);
            }
            else if (string.Compare(outputExtension, ".bmp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Bmp, 100);
            }
            else if (string.Compare(outputExtension, ".gif", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Gif, 100);
            }
            else if (string.Compare(outputExtension, ".ico", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Ico, 100);
            }
            else if (string.Compare(outputExtension, ".jpeg", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Jpeg, 100);
            }
            else if (string.Compare(outputExtension, ".png", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Png, 100);
            }
            else if (string.Compare(outputExtension, ".wbmp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Wbmp, 100);
            }
            else if (string.Compare(outputExtension, ".webp", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Webp, 100);
            }
            else if (string.Compare(outputExtension, ".pkm", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Pkm, 100);
            }
            else if (string.Compare(outputExtension, ".ktx", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Ktx, 100);
            }
            else if (string.Compare(outputExtension, ".astc", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Astc, 100);
            }
            else if (string.Compare(outputExtension, ".dng", StringComparison.OrdinalIgnoreCase) == 0)
            {
                ExportImage(context, path, containerView, SKEncodedImageFormat.Dng, 100);
            }
        }

        public async void ImportFile()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg Files", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.AllowMultiple = true;
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                foreach (var path in result)
                {
                    ImportSvg(this, path);
                }
            }
        }

        public async void ExportFile()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Pdf Files", Extensions = { "pdf" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Xps Files", Extensions = { "xps" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg Files", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Skp Files", Extensions = { "skp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Bmp Files", Extensions = { "bmp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Gif Files", Extensions = { "gif" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Ico Files", Extensions = { "ico" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Jpeg Files", Extensions = { "jpeg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Png Files", Extensions = { "png" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Wbmp Files", Extensions = { "wbmp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Webp Files", Extensions = { "webp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Pkm Files", Extensions = { "pkm" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Ktx Files", Extensions = { "ktx" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Astc Files", Extensions = { "astc" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Dng Files", Extensions = { "dng" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = ContainerView.Title;
            dlg.DefaultExtension = "pdf";
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                var path = result;
                Export(this, path, ContainerView);
            }
        }

        private static string FormatXml(string xml)
        {
            var sb = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            settings.NewLineOnAttributes = false;

            using (var writer = XmlWriter.Create(sb, settings))
            {
                element.Save(writer);
            }

            return sb.ToString();
        }

        private string CopySvgImpl(IToolContext context, IContainerView containerView)
        {
            using (var stream = new MemoryStream())
            {
                using (var wstream = new SKManagedWStream(stream))
                using (var writer = new SKXmlStreamWriter(wstream))
                using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)containerView.Width, (int)containerView.Height), writer))
                {
                    if (containerView.SelectionState?.Shapes?.Count > 0)
                    {
                        using (var skiaSelectedPresenter = new ExportSelectedPresenter(context, containerView))
                        {
                            skiaSelectedPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                        }
                    }
                    else
                    {
                        using (var skiaContainerPresenter = new ExportContainerPresenter(context, containerView))
                        {
                            skiaContainerPresenter.Draw(canvas, containerView.Width, containerView.Height, 0, 0, 1.0, 1.0);
                        }
                    }
                }

                stream.Position = 0;
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var xml = reader.ReadToEnd();
                    return FormatXml(xml);
                }
            }
        }

        public async void CopySvg()
        {
            try
            {
                var text = CopySvgImpl(this, ContainerView);
                if (!string.IsNullOrEmpty(text))
                {
                    await Application.Current.Clipboard.SetTextAsync(text);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        public void ToSvg(TextBox textBox)
        {
            var text = CopySvgImpl(this, ContainerView);
            if (!string.IsNullOrEmpty(text))
            {
                textBox.Text = text;
            }
        }

        private string CopySvgPathDataImpl()
        {
            if (ContainerView.SelectionState?.Shapes != null)
            {
                return PathConverter?.ToSvgPathData(this, ContainerView.SelectionState?.Shapes);
            }
            return null;
        }

        public async void CopySvgPathData()
        {
            try
            {
                var text = CopySvgPathDataImpl();
                if (!string.IsNullOrEmpty(text))
                {
                    await Application.Current.Clipboard.SetTextAsync(text);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        private void PasteSvgPathDataImpl(string text)
        {
            var path = PathConverter?.ToPathShape(this, text);
            if (path != null)
            {
                ContainerView?.CurrentContainer?.Shapes.Add(path);
                ContainerView?.CurrentContainer?.MarkAsDirty(true);

                ContainerView?.SelectionState?.Dehover();
                ContainerView?.SelectionState?.Clear();

                path.Select(ContainerView?.SelectionState);

                ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public async void PasteSvgPathData()
        {
            try
            {
                var text = await Application.Current.Clipboard.GetTextAsync();
                if (!string.IsNullOrEmpty(text))
                {
                    PasteSvgPathDataImpl(text);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        public void ToSvgPathData(TextBox textBox)
        {
            var text = CopySvgPathDataImpl();
            if (!string.IsNullOrEmpty(text))
            {
                textBox.Text = text;
            }
        }

        public void FromSvgPathData(TextBox textBox)
        {
            var text = textBox.Text;
            if (!string.IsNullOrEmpty(text))
            {
                PasteSvgPathDataImpl(text);
            }
        }

        public void PathOp(string parameter)
        {
            if (Enum.TryParse<PathOp>(parameter, true, out var op) == true)
            {
                var path = PathConverter?.Op(this, op, ContainerView?.SelectionState?.Shapes);
                if (path != null)
                {
                    Selection.Delete(this);

                    ContainerView?.CurrentContainer?.Shapes.Add(path);
                    ContainerView?.CurrentContainer?.MarkAsDirty(true);

                    ContainerView?.SelectionState?.Dehover();
                    ContainerView?.SelectionState?.Clear();

                    path.Select(ContainerView?.SelectionState);

                    ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
        }

        public void Exit()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                desktopLifetime.Shutdown();
            }
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
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };
            rectangle.StartPoint.Owner = rectangle;
            rectangle.Point.Owner = rectangle;

            var text = new TextShape(new PointShape(0, 0, context.PointTemplate), new PointShape(30, 30, context.PointTemplate))
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = new Text("&"),
                StyleId = context.StyleLibrary?.CurrentItem?.Title
            };
            text.StartPoint.Owner = text;
            text.Point.Owner = text;

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

        private Window GetWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }
    }
}
