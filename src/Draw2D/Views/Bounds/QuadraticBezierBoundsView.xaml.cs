using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Bounds
{
    public class QuadraticBezierBoundsView : UserControl
    {
        public QuadraticBezierBoundsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
