
namespace Draw2D.ViewModels;

public interface INode
{
    string Id { get; set; }
    string Name { get; set; }
    string Title { get; set; }
    object Owner { get; set; }
}