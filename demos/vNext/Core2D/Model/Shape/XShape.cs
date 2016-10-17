using System.Collections.Immutable;

namespace Core2D.Model
{
    public class XShape : ObservableObject
    {
        private string _name;
        private ImmutableArray<XProperty> _properties;
        private XLayer _owner;

        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        public ImmutableArray<XProperty> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        public XLayer Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }
    }
}
