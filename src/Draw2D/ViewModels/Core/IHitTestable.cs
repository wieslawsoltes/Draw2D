using Draw2D.Input;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels
{
    public interface IHitTestable
    {
        IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius, double scale, Modifier modifier);
    }
}
