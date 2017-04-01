using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Draw2D.Editor;
using Draw2D.ViewModels.Containers;

namespace Draw2D.PathDemo.Controls
{
    public class PathCanvas : Canvas
    {
        private Modifier GetModifier()
        {
            Modifier modifier = Modifier.None;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
                modifier |= Modifier.Alt;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                modifier |= Modifier.Control;

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                modifier |= Modifier.Shift;

            return modifier;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                var point = e.GetPosition(this);
                vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier());
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                var point = e.GetPosition(this);
                vm.CurrentTool.LeftUp(vm, point.X, point.Y, GetModifier());
            }
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                var point = e.GetPosition(this);
                vm.CurrentTool.RightDown(vm, point.X, point.Y, GetModifier());
            }
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                var point = e.GetPosition(this);
                vm.CurrentTool.RightUp(vm, point.X, point.Y, GetModifier());
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                var point = e.GetPosition(this);
                vm.CurrentTool.Move(vm, point.X, point.Y, GetModifier());
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.Presenter.Draw(dc, vm);
                vm.Presenter.DrawHelpers(dc, vm);
            }
        }
    }
}
