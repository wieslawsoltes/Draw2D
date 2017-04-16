// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Input;
using Draw2D.Editor;
using Draw2D.ViewModels.Containers;

namespace Draw2D.Avalonia.Controls
{
    public class ShapesContainerInputView : Border
    {
        public ShapesContainerInputView()
        {
            PointerPressed += (sender, e) => HandlePointerPressed(e);
            PointerReleased += (sender, e) => HandlePointerReleased(e);
            PointerMoved += (sender, e) => HandlePointerMoved(e);
        }

        private Modifier GetModifier(InputModifiers inputModifiers)
        {
            Modifier modifier = Modifier.None;

            if (inputModifiers.HasFlag(InputModifiers.Alt))
            {
                modifier |= Modifier.Alt;
            }

            if (inputModifiers.HasFlag(InputModifiers.Control))
            {
                modifier |= Modifier.Control;
            }

            if (inputModifiers.HasFlag(InputModifiers.Shift))
            {
                modifier |= Modifier.Shift;
            }

            return modifier;
        }

        private void HandlePointerPressed(PointerPressedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is ShapesContainerViewModel vm)
                {
                    var point = e.GetPosition(Child);
                    vm.CurrentTool.LeftDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    Child.InvalidateVisual();
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is ShapesContainerViewModel vm)
                {
                    var point = e.GetPosition(Child);
                    vm.CurrentTool.RightDown(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    Child.InvalidateVisual();
                }
            }
        }

        private void HandlePointerReleased(PointerReleasedEventArgs e)
        {
            if (e.MouseButton == MouseButton.Left)
            {
                if (this.DataContext is ShapesContainerViewModel vm)
                {
                    var point = e.GetPosition(Child);
                    vm.CurrentTool.LeftUp(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    Child.InvalidateVisual();
                }
            }
            else if (e.MouseButton == MouseButton.Right)
            {
                if (this.DataContext is ShapesContainerViewModel vm)
                {
                    var point = e.GetPosition(Child);
                    vm.CurrentTool.RightUp(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                    Child.InvalidateVisual();
                }
            }
        }

        private void HandlePointerMoved(PointerEventArgs e)
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                var point = e.GetPosition(Child);
                vm.CurrentTool.Move(vm, point.X, point.Y, GetModifier(e.InputModifiers));
                Child.InvalidateVisual();
            }
        }
    }
}
