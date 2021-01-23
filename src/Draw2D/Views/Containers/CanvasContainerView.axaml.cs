using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Containers
{
    public class CanvasContainerView : UserControl
    {
        public CanvasContainerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
