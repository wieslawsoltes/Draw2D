using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Containers
{
    public interface IContainerExporter
    {
        void Export(IToolContext context, string path, IContainerView containerView);
    }
}
