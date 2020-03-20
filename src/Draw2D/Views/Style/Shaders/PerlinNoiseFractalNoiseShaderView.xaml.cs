using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Style.Shaders
{
    public class PerlinNoiseFractalNoiseShaderView : UserControl
    {
        public PerlinNoiseFractalNoiseShaderView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
