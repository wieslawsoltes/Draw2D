using System.Collections.Generic;
using Svg;

namespace SvgDemo
{
    public class Element
    {
        public SvgElement Original { get; set; }
        public Element Parent { get; set; }
        public string Name { get; set; }
        public List<Element> Children { get; set; }

        public Element()
        {
            Children = new List<Element>();
        }

        public Element(SvgElement original, Element parent, string name)
            : this()
        {
            Original = original;
            Parent = parent;
            Name = name;
        }
    }
}
