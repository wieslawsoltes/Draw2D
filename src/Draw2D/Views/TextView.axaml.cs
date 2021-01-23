using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views
{
    public class TextView : UserControl
    {
        public TextView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
