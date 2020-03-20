using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Containers
{
    public interface ISvgConverter
    {
        string ConvertToSvgDocument(IToolContext context, IContainerView containerView);
    }
}
