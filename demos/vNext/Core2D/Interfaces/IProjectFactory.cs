using Core2D.Model;

namespace Core2D.Factories
{
    public interface IProjectFactory
    {
        XProperty CreateProperty(XShape owner, string name, string value);
        XShape CreateShape(XLayer owner, string name);
        XLayer CreateLayer(XPage owner, string name);
        XPage CreatePage(XDocument owner, string name);
        XDocument CreateDocument(XProject owner, string name);
        XProject CreateProject(string name);
    }
}