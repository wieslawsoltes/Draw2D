using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Containers
{
    public interface IContainerImporter
    {
        void Import(IToolContext context, string path, IContainerView containerView);
    }
}
