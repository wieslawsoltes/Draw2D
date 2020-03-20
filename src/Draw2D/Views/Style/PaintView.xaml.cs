using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style
{
    public class PaintView : UserControl
    {
        public PaintView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
