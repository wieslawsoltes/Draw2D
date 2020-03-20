using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style
{
    public class ArgbColorView : UserControl
    {
        public ArgbColorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
