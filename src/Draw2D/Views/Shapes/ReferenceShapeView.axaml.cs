using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Shapes
{
    public class ReferenceShapeView : UserControl
    {
        public ReferenceShapeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
