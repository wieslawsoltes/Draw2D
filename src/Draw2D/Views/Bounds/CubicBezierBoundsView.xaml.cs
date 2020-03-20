using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Bounds
{
    public class CubicBezierBoundsView : UserControl
    {
        public CubicBezierBoundsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
