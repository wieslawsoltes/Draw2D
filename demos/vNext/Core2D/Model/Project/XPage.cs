using System.Collections.Immutable;

namespace Core2D.Model
{
    public class XPage : ObservableObject
    {
        private string _name;
        private ImmutableArray<XLayer> _layers;
        private XDocument _owner;

        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        public ImmutableArray<XLayer> Layers
        {
            get { return _layers; }
            set { Update(ref _layers, value); }
        }

        public XDocument Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }
    }
}
