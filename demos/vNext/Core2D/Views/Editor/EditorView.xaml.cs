using System.Windows.Controls;

namespace Core2D.Views
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
            Loaded += (sender, e) => Focus();
        }
    }
}
