using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.ImageFilters
{
    public class DilateImageFilterView : UserControl
    {
        public DilateImageFilterView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
