using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Tools
{
    public class SelectionToolView : UserControl
    {
        public SelectionToolView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
