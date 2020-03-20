using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Bounds
{
    public class PointBoundsView : UserControl
    {
        public PointBoundsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
