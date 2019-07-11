// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Draw2D.Presenters;
using Draw2D.Serializer;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor
{
    [DataContract(IsReference = true)]
    public class EditorToolContext : ToolContext, IEditorToolContext
    {
        public new static EditMode[] EditModeValues { get; } = (EditMode[])Enum.GetValues(typeof(EditMode));

        private IContainerFactory _containerFactory;
        private IAvaloniaXamlConverter _avaloniaXamlConverter;
        private IContainerImporter _containerImporter;
        private IContainerExporter _containerExporter;
        private ISvgConverter _svgConverter;
        private ISelection _selection;
        private string _currentDirectory;
        private IList<string> _files;

        [IgnoreDataMember]
        public IContainerFactory ContainerFactory
        {
            get => _containerFactory;
            set => Update(ref _containerFactory, value);
        }

        [IgnoreDataMember]
        public IAvaloniaXamlConverter AvaloniaXamlConverter
        {
            get => _avaloniaXamlConverter;
            set => Update(ref _avaloniaXamlConverter, value);
        }

        [IgnoreDataMember]
        public IContainerImporter ContainerImporter
        {
            get => _containerImporter;
            set => Update(ref _containerImporter, value);
        }

        [IgnoreDataMember]
        public IContainerExporter ContainerExporter
        {
            get => _containerExporter;
            set => Update(ref _containerExporter, value);
        }

        [IgnoreDataMember]
        public ISvgConverter SvgConverter
        {
            get => _svgConverter;
            set => Update(ref _svgConverter, value);
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

        private Window GetWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
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
               _documentContainer?. ContainerView?.SelectionState.Clear();

                _documentContainer?.ContainerViews.Add(containerView);
                _documentContainer?.ContainerView = containerView;

                _documentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        public void CloseContainerView(IContainerView containerView)
        {
            if (containerView != null)
            {
                int index = ContainerViews.IndexOf(containerView);
                if (index >= 0)
                {
                    _documentContainer?.ContainerViews.Remove(containerView);

                    int count = ContainerViews.Count;
                    if (count > 0)
                    {
                        int selectedIndex = (count == 1 || index == 0) ? 0 : index - 1;
                        _documentContainer?.ContainerView = _documentContainer?.ContainerViews[selectedIndex];
                    }
                    else
                    {
                        _documentContainer?.ContainerView = null;
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

        public void OpenDocument(string path)
        {
            var containerView = JsonSerializer.FromJsonFile<ContainerView>(path);
            if (containerView != null)
            {
                InitContainerView(containerView);
                AddContainerView(containerView);
            }
        }

        public void SaveDocument(string path)
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

        public async void OpenDocumentContainer()
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
                    OpenDocument(path);
                }
            }
        }

        public async void SaveDocumentContainerAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = DocumentContainer.Title;
            dlg.DefaultExtension = "json";
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                var path = result;
                SaveDocument(path);
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

        public async void ImportFile()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Supported Files", Extensions = { "svg", "skp", "bmp", "gif", "ico", "jpeg", "jpg", "png", "wbmp", "webp", "pkm", "ktx", "astc", "dng" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Svg Files", Extensions = { "svg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Skp Files", Extensions = { "skp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Bmp Files", Extensions = { "bmp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Gif Files", Extensions = { "gif" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Ico Files", Extensions = { "ico" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Jpeg Files", Extensions = { "jpeg", "jpg" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Png Files", Extensions = { "png" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Wbmp Files", Extensions = { "wbmp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Webp Files", Extensions = { "webp" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Pkm Files", Extensions = { "pkm" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Ktx Files", Extensions = { "ktx" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Astc Files", Extensions = { "astc" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "Dng Files", Extensions = { "dng" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.AllowMultiple = true;
            var result = await dlg.ShowAsync(GetWindow());
            if (result != null)
            {
                foreach (var path in result)
                {
                    ContainerImporter?.Import(this, path, ContainerView);
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
            dlg.Filters.Add(new FileDialogFilter() { Name = "Jpeg Files", Extensions = { "jpeg", "jpg" } });
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
                ContainerExporter?.Export(this, path, ContainerView);
            }
        }

        public async void CopySvgDocument()
        {
            try
            {
                var text = SvgConverter?.ConvertToSvgDocument(this, ContainerView);
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

        public async void CopySvgPathData()
        {
            try
            {
                if (ContainerView.SelectionState?.Shapes != null)
                {
                    var text = PathConverter?.ToSvgPathData(this, ContainerView.SelectionState?.Shapes);
                    if (!string.IsNullOrEmpty(text))
                    {
                        await Application.Current.Clipboard.SetTextAsync(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        public async void PasteSvgPathData()
        {
            try
            {
                var text = await Application.Current.Clipboard.GetTextAsync();
                if (!string.IsNullOrEmpty(text))
                {
                    var path = PathConverter?.ToPathShape(this, text);
                    if (path != null)
                    {
                        path.Owner = ContainerView?.CurrentContainer;
                        ContainerView?.CurrentContainer?.Shapes.Add(path);
                        ContainerView?.CurrentContainer?.MarkAsDirty(true);

                        ContainerView?.SelectionState?.Dehover();
                        ContainerView?.SelectionState?.Clear();

                        path.Select(ContainerView?.SelectionState);

                        ContainerView?.InputService?.Redraw?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.Message);
                Log.WriteLine(ex.StackTrace);
            }
        }

        public async void CopyGeometryDrawing()
        {
            try
            {
                var sb = new StringBuilder();
                _avaloniaXamlConverter?.ConvertToGeometryDrawing(this, this.ContainerView, sb, "");
                var text = sb.ToString();
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

        public async void CopyDrawingGroup()
        {
            try
            {
                var sb = new StringBuilder();
                _avaloniaXamlConverter?.ConvertToDrawingGroup(this, this.ContainerView, sb, "");
                var text = sb.ToString();
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

        public async void CopyDrawingPresenter()
        {
            try
            {
                var sb = new StringBuilder();
                _avaloniaXamlConverter?.ConvertToDrawingPresenter(this, this.ContainerView, sb, "");
                var text = sb.ToString();
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

        public async void CopyPath()
        {
            try
            {
                var sb = new StringBuilder();
                _avaloniaXamlConverter?.ConvertToPath(this, this.ContainerView, sb, "");
                var text = sb.ToString();
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

        public async void CopyCanvas()
        {
            try
            {
                var sb = new StringBuilder();
                _avaloniaXamlConverter?.ConvertToCanvas(this, this.ContainerView, sb, "");
                var text = sb.ToString();
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

        public void PathOp(string parameter)
        {
            if (Enum.TryParse<PathOp>(parameter, true, out var op) == true)
            {
                var path = PathConverter?.Op(this, op, ContainerView?.SelectionState?.Shapes);
                if (path != null)
                {
                    Selection.Delete(this);

                    path.Owner = ContainerView?.CurrentContainer;
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
    }
}
