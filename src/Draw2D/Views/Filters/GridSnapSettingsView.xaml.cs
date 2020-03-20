using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Filters
{
    public class GridSnapSettingsView : UserControl
    {
        public GridSnapSettingsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
