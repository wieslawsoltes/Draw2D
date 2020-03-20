
namespace Draw2D.ViewModels
{
    public interface IDirty
    {
        bool IsDirty { get; set; }
        void MarkAsDirty(bool isDirty);
        bool IsTreeDirty();
        void Invalidate();
    }
}
