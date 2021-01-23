using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Tools
{
    public class PointToolSettingsView : UserControl
    {
        public PointToolSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
