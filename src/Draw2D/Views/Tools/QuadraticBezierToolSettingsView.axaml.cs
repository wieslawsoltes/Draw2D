using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Tools
{
    public class QuadraticBezierToolSettingsView : UserControl
    {
        public QuadraticBezierToolSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
