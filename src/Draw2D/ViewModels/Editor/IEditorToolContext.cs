using System.Collections.Generic;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor
{
    public interface IEditorToolContext : IToolContext
    {
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
}
