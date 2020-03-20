using System.Text;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Containers
{
    public interface IAvaloniaXamlConverter
    {
        void ConvertToGeometryDrawing(IToolContext context, IContainerView containerView, StringBuilder sb, string indent);
        void ConvertToDrawingGroup(IToolContext context, IContainerView containerView, StringBuilder sb, string indent);
        void ConvertToDrawingPresenter(IToolContext context, IContainerView containerView, StringBuilder sb, string indent);
        void ConvertToPath(IToolContext context, IContainerView containerView, StringBuilder sb, string indent);
        void ConvertToCanvas(IToolContext context, IContainerView containerView, StringBuilder sb, string indent);
    }
}
