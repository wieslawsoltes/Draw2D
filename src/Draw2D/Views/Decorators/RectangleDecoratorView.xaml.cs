using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Decorators
{
    public class RectangleDecoratorView : UserControl
    {
        public RectangleDecoratorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
