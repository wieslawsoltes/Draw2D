using System.Collections.Immutable;

namespace Core2D.Model
{
    public class XDocument : ObservableObject
    {
        private string _name;
        private ImmutableArray<XPage> _pages;
        private XProject _owner;

        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        public ImmutableArray<XPage> Pages
        {
            get { return _pages; }
            set { Update(ref _pages, value); }
        }

        public XProject Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }
    }
}
