using System;

namespace Draw2D.ViewModels.Containers
{
    public interface IContainerPresenter : IDisposable
    {
        void Draw(object context, double width, double height, double dx, double dy, double zx, double zy, double renderScaling);
    }
}
