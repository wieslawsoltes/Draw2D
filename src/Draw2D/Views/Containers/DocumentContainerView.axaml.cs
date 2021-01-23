using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Containers
{
    public class DocumentContainerView : UserControl
    {
        public DocumentContainerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
