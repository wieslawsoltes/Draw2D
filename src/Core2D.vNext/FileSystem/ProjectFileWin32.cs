using Microsoft.Win32;

namespace Core2D.FileSystem
{
    public class ProjectFileWin32 : IFile
    {
        public string GetOpenFileName()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Project (*.project)|*.project|All Files (*.*)|*.*"
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }

        public string GetSaveFileName(string fileName)
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All Files (*.*)|*.*",
                FileName = fileName
            };
            return dlg.ShowDialog() == true ? dlg.FileName : null;
        }
    }
}
