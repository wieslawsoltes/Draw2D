using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Decorators
{
    public class PointDecoratorView : UserControl
    {
        public PointDecoratorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
