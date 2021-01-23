using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.ImageFilters
{
    public class AlphaThresholdImageFilterView : UserControl
    {
        public AlphaThresholdImageFilterView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
