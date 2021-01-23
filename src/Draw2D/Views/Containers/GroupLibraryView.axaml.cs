using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Containers
{
    public class GroupLibraryView : UserControl
    {
        public GroupLibraryView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
