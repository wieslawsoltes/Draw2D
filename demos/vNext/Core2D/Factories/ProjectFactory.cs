using System.Collections.Immutable;
using Core2D.Model;

namespace Core2D.Factories
{
    public class ProjectFactory : IProjectFactory
    {
        public XProperty CreateProperty(XShape owner, string name, string value)
        {
            return new XProperty()
            {
                Owner = owner,
                Name = name,
                Value = value,
            };
        }

        public XShape CreateShape(XLayer owner, string name)
        {
            return new XShape()
            {
                Owner = owner,
                Name = name,
                Properties = ImmutableArray.Create<XProperty>()
            };
        }

        public XLayer CreateLayer(XPage owner, string name)
        {
            return new XLayer()
            {
                Owner = owner,
                Name = name,
                Shapes = ImmutableArray.Create<XShape>()
            };
        }

        public XPage CreatePage(XDocument owner, string name)
        {
            return new XPage()
            {
                Owner = owner,
                Name = name,
                Layers = ImmutableArray.Create<XLayer>()
            };
        }

        public XDocument CreateDocument(XProject owner, string name)
        {
            return new XDocument()
            {
                Owner = owner,
                Name = name,
                Pages = ImmutableArray.Create<XPage>()
            };
        }

        public XProject CreateProject(string name)
        {
            return new XProject()
            {
                Name = name,
                Documents = ImmutableArray.Create<XDocument>()
            };
        }
    }
}
