using System.Collections.Immutable;

namespace Core2D.Model
{
    public class XProject : ObservableObject
    {
        private string _name;
        private ImmutableArray<XDocument> _documents;

        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        public ImmutableArray<XDocument> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }
    }
}
