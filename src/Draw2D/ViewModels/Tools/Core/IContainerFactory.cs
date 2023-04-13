using Draw2D.ViewModels.Containers;

namespace Draw2D.ViewModels.Tools;

public interface IContainerFactory
{
    IStyleLibrary CreateStyleLibrary();
    IGroupLibrary CreateGroupLibrary();
    IToolContext CreateToolContext();
    IContainerView CreateContainerView(string title);
    IDocumentContainer CreateDocumentContainer(string title);
}