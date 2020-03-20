using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Shapes
{
    public class LineShapeView : UserControl
    {
        public LineShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
