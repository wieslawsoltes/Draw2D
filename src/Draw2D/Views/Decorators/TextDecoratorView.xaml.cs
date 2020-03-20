using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Decorators
{
    public class TextDecoratorView : UserControl
    {
        public TextDecoratorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
