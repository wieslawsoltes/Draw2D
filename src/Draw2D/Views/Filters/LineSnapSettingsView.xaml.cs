using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Filters
{
    public class LineSnapSettingsView : UserControl
    {
        public LineSnapSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
