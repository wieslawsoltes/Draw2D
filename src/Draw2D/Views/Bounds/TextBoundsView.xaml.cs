using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Bounds
{
    public class TextBoundsView : UserControl
    {
        public TextBoundsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
