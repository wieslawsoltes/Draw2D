using System.Windows;
using Core2D.Clipboard;
using Core2D.Factories;
using Core2D.FileSystem;
using Core2D.History;
using Core2D.Serializer;
using Core2D.ViewModels;
using Core2D.Views;

namespace Core2D
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using (var vm = new EditorViewModel(new VirtualClipboard(), new JsonSerializerNewtonsoft(), new StackHistory(), new ProjectFactory(), new ProjectFileWin32()))
            {
                var mainView = new MainView { DataContext = vm }.ShowDialog();
            }
        }
    }
}
