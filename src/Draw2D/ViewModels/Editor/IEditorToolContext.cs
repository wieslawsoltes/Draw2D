using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor;

public interface IEditorToolContext : IToolContext
{
    IRelayCommand<IContainerView> InitContainerViewCommand { get; }
    IRelayCommand<IContainerView> AddContainerViewCommand { get; }
    IRelayCommand<IContainerView> CloseContainerViewCommand { get; }
    IRelayCommand<string> NewContainerViewCommand { get; }
    IRelayCommand<string> OpenDocumentCommand { get; }
    IRelayCommand<string> SaveDocumentCommand { get; }
    IRelayCommand<string> OpenStylesCommand { get; }
    IRelayCommand<string> SaveStylesCommand { get; }
    IRelayCommand<string> OpenGroupsCommand { get; }
    IRelayCommand<string> SaveGroupsCommand { get; }
    IRelayCommand OpenDocumentContainerCommand { get; }
    IRelayCommand SaveDocumentContainerAsCommand { get; }
    IRelayCommand OpenStyleLibraryCommand { get; }
    IRelayCommand SaveStyleLibraryAsCommand { get; }
    IRelayCommand OpenGroupLibraryCommand { get; }
    IRelayCommand SaveGroupLibraryAsCommand { get; }
    IRelayCommand<string> AddFilesCommand { get; }
    IRelayCommand ClearFilesCommand { get; }
    IRelayCommand ImportFileCommand { get; }
    IRelayCommand ExportFileCommand { get; }
    IRelayCommand CopySvgDocumentCommand { get; }
    IRelayCommand CopySvgPathDataCommand { get; }
    IRelayCommand PasteSvgPathDataCommand { get; }
    IRelayCommand CopyGeometryDrawingCommand { get; }
    IRelayCommand CopyDrawingGroupCommand { get; }
    IRelayCommand CopyDrawingPresenterCommand { get; }
    IRelayCommand CopyPathCommand { get; }
    IRelayCommand CopyCanvasCommand { get; }
    IRelayCommand<string> PathOpCommand { get; }
    IRelayCommand ExitCommand { get; }
    IContainerFactory ContainerFactory { get; set; }
    IAvaloniaXamlConverter AvaloniaXamlConverter { get; set; }
    IContainerImporter ContainerImporter { get; set; }
    IContainerExporter ContainerExporter { get; set; }
    ISvgConverter SvgConverter { get; set; }
    ISelection Selection { get; set; }
    string CurrentDirectory { get; set; }
    IList<string> Files { get; set; }
    void InitContainerView(IContainerView containerView);
    void AddContainerView(IContainerView containerView);
    void CloseContainerView(IContainerView containerView);
    void NewContainerView(string title);
    void OpenDocument(string path);
    void SaveDocument(string path);
    void OpenStyles(string path);
    void SaveStyles(string path);
    void OpenGroups(string path);
    void SaveGroups(string path);
    void OpenDocumentContainer();
    void SaveDocumentContainerAs();
    void OpenStyleLibrary();
    void SaveStyleLibraryAs();
    void OpenGroupLibrary();
    void SaveGroupLibraryAs();
    void AddFiles(string path);
    void ClearFiles();
    void ImportFile();
    void ExportFile();
    void CopySvgDocument();
    void CopySvgPathData();
    void PasteSvgPathData();
    void CopyGeometryDrawing();
    void CopyDrawingGroup();
    void CopyDrawingPresenter();
    void CopyPath();
    void CopyCanvas();
    void PathOp(string parameter);
    void Exit();
}
