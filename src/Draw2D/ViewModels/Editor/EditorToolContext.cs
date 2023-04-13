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

namespace Draw2D.Editor;

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
            DocumentContainer.ContainerView?.SelectionState.Clear();

            DocumentContainer.ContainerViews.Add(containerView);
            DocumentContainer.ContainerView = containerView;

            DocumentContainer.ContainerView?.InputService?.Redraw?.Invoke();
        }
    }

    public void CloseContainerView(IContainerView containerView)
    {
        if (containerView != null)
        {
            int index = DocumentContainer.ContainerViews.IndexOf(containerView);
            if (index >= 0)
            {
                DocumentContainer?.ContainerViews.Remove(containerView);

                int count = DocumentContainer.ContainerViews.Count;
                if (count > 0)
                {
                    int selectedIndex = (count == 1 || index == 0) ? 0 : index - 1;
                    DocumentContainer.ContainerView = DocumentContainer.ContainerViews[selectedIndex];
                }
                else
                {
                    DocumentContainer.ContainerView = null;
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
        var documentContainer = JsonSerializer.FromJsonFile<IDocumentContainer>(path);
        if (documentContainer != null)
        {
            foreach (var containerView in documentContainer.ContainerViews)
            {
                InitContainerView(containerView);
            }
            DocumentContainer = documentContainer;
        }
    }

    public void SaveDocument(string path)
    {
        if (DocumentContainer != null)
        {
            JsonSerializer.ToJsonFile(path, DocumentContainer);
        }
    }

    public void OpenStyles(string path)
    {
        var styleLibrary = JsonSerializer.FromJsonFile<IStyleLibrary>(path);
        if (styleLibrary != null)
        {
            DocumentContainer.StyleLibrary = styleLibrary;
            DocumentContainer.StyleLibrary.UpdateCache();
        }
    }

    public void SaveStyles(string path)
    {
        if (DocumentContainer.StyleLibrary != null)
        {
            JsonSerializer.ToJsonFile(path, DocumentContainer.StyleLibrary);
        }
    }

    public void OpenGroups(string path)
    {
        var groupLibrary = JsonSerializer.FromJsonFile<IGroupLibrary>(path);
        if (groupLibrary != null)
        {
            DocumentContainer.GroupLibrary = groupLibrary;
            DocumentContainer.GroupLibrary.UpdateCache();
        }
    }

    public void SaveGroups(string path)
    {
        if (DocumentContainer.GroupLibrary != null)
        {
            JsonSerializer.ToJsonFile(path, DocumentContainer.GroupLibrary);
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
                ContainerImporter?.Import(this, path, DocumentContainer.ContainerView);
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
        dlg.InitialFileName = DocumentContainer.ContainerView.Title;
        dlg.DefaultExtension = "pdf";
        var result = await dlg.ShowAsync(GetWindow());
        if (result != null)
        {
            var path = result;
            ContainerExporter?.Export(this, path, DocumentContainer.ContainerView);
        }
    }

    public async void CopySvgDocument()
    {
        try
        {
            var text = SvgConverter?.ConvertToSvgDocument(this, DocumentContainer.ContainerView);
            if (!string.IsNullOrEmpty(text))
            {
                await Application.Current.Clipboard.SetTextAsync(text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public async void CopySvgPathData()
    {
        try
        {
            if (DocumentContainer.ContainerView.SelectionState?.Shapes != null)
            {
                var text = PathConverter?.ToSvgPathData(this, DocumentContainer.ContainerView.SelectionState?.Shapes);
                if (!string.IsNullOrEmpty(text))
                {
                    await Application.Current.Clipboard.SetTextAsync(text);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
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
                    path.Owner = DocumentContainer.ContainerView?.CurrentContainer;
                    DocumentContainer.ContainerView?.CurrentContainer?.Shapes.Add(path);
                    DocumentContainer.ContainerView?.CurrentContainer?.MarkAsDirty(true);

                    DocumentContainer.ContainerView?.SelectionState?.Dehover();
                    DocumentContainer.ContainerView?.SelectionState?.Clear();

                    path.Select(DocumentContainer.ContainerView?.SelectionState);

                    DocumentContainer.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public async void CopyGeometryDrawing()
    {
        try
        {
            var sb = new StringBuilder();
            _avaloniaXamlConverter?.ConvertToGeometryDrawing(this, DocumentContainer.ContainerView, sb, "");
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                await Application.Current.Clipboard.SetTextAsync(text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public async void CopyDrawingGroup()
    {
        try
        {
            var sb = new StringBuilder();
            _avaloniaXamlConverter?.ConvertToDrawingGroup(this, DocumentContainer.ContainerView, sb, "");
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                await Application.Current.Clipboard.SetTextAsync(text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public async void CopyDrawingPresenter()
    {
        try
        {
            var sb = new StringBuilder();
            _avaloniaXamlConverter?.ConvertToDrawingPresenter(this, DocumentContainer.ContainerView, sb, "");
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                await Application.Current.Clipboard.SetTextAsync(text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public async void CopyPath()
    {
        try
        {
            var sb = new StringBuilder();
            _avaloniaXamlConverter?.ConvertToPath(this, DocumentContainer.ContainerView, sb, "");
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                await Application.Current.Clipboard.SetTextAsync(text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public async void CopyCanvas()
    {
        try
        {
            var sb = new StringBuilder();
            _avaloniaXamlConverter?.ConvertToCanvas(this, DocumentContainer.ContainerView, sb, "");
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
            {
                await Application.Current.Clipboard.SetTextAsync(text);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.Message);
            System.Diagnostics.Debug.WriteLine(ex.StackTrace);
        }
    }

    public void PathOp(string parameter)
    {
        if (Enum.TryParse<PathOp>(parameter, true, out var op) == true)
        {
            var path = PathConverter?.Op(this, op, DocumentContainer.ContainerView?.SelectionState?.Shapes);
            if (path != null)
            {
                Selection.Delete(this);

                path.Owner = DocumentContainer.ContainerView?.CurrentContainer;
                DocumentContainer.ContainerView?.CurrentContainer?.Shapes.Add(path);
                DocumentContainer.ContainerView?.CurrentContainer?.MarkAsDirty(true);

                DocumentContainer.ContainerView?.SelectionState?.Dehover();
                DocumentContainer.ContainerView?.SelectionState?.Clear();

                path.Select(DocumentContainer.ContainerView?.SelectionState);

                DocumentContainer.ContainerView?.InputService?.Redraw?.Invoke();
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