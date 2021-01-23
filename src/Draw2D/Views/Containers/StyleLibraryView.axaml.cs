using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Containers
{
    public class StyleLibraryView : UserControl
    {
        public StyleLibraryView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
