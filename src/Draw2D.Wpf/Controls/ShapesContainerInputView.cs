using System;
using System.Windows.Controls;
using System.Windows.Input;
using Draw2D.Editor;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Wpf.Controls
{
    public class ShapesContainerInputView : Border
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

            var point = e.GetPosition(Child);
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier());
                Child.InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            var point = e.GetPosition(Child);
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                vm.CurrentTool.LeftUp(vm, point.X, point.Y, GetModifier());
                Child.InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            var point = e.GetPosition(Child);
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                vm.CurrentTool.RightDown(vm, point.X, point.Y, GetModifier());
                Child.InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);

            var point = e.GetPosition(Child);
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                vm.CurrentTool.RightUp(vm, point.X, point.Y, GetModifier());
                Child.InvalidateVisual();
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            var point = e.GetPosition(Child);
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                vm.CurrentTool.Move(vm, point.X, point.Y, GetModifier());
                Child.InvalidateVisual();
            }
        }
    }
}
