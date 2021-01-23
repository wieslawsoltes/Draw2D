using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.ImageFilters
{
    public class ComposeImageFilterView : UserControl
    {
        public ComposeImageFilterView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
