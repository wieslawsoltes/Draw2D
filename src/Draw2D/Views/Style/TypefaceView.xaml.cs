using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style
{
    public class TypefaceView : UserControl
    {
        public TypefaceView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
