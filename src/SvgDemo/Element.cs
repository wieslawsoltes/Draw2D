using System;
using System.Collections.Generic;
using Svg;

namespace SvgDemo
{
    public class Element
    {
        public SvgElement Original { get; set; }
        public Element Parent { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<Element> Children { get; set; }

        public Element()
        {
            Attributes = new Dictionary<string, object>();
            Children = new List<Element>();
        }

        public Element(SvgElement original, Element parent, string name, Type type)
            : this()
        {
            Original = original;
            Parent = parent;
            Name = name;
            Type = type;
        }
    }
}
