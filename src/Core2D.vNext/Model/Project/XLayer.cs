using System.Collections.Immutable;

namespace Core2D.Model
{
    public class XLayer : ObservableObject
    {
        private string _name;
        private ImmutableArray<XShape> _shapes;
        private XPage _owner;

        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        public ImmutableArray<XShape> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        public XPage Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }
    }
}
