using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.ImageFilters
{
    public class PaintImageFilterView : UserControl
    {
        public PaintImageFilterView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
