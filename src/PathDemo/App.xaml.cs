using System.Windows;
using PathDemo.Views;

namespace PathDemo
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new MainView();
            window.ShowDialog();
        }
    }
}
