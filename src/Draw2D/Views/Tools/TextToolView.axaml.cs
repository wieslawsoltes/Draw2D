using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Tools
{
    public class TextToolView : UserControl
    {
        public TextToolView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
