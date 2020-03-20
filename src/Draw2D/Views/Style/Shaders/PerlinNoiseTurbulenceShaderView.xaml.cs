using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.Shaders
{
    public class PerlinNoiseTurbulenceShaderView : UserControl
    {
        public PerlinNoiseTurbulenceShaderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
