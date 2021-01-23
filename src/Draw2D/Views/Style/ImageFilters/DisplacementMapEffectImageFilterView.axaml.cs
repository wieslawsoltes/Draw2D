using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.ImageFilters
{
    public class DisplacementMapEffectImageFilterView : UserControl
    {
        public DisplacementMapEffectImageFilterView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
