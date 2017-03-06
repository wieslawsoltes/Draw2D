
namespace Core2D.FileSystem
{
    public interface IFile
    {
        string GetOpenFileName();
        string GetSaveFileName(string fileName);
    }
}
