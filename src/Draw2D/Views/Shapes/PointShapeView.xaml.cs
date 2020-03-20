using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Shapes
{
    public class PointShapeView : UserControl
    {
        public PointShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
