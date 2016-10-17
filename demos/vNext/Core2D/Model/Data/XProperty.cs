
namespace Core2D.Model
{
    public class XProperty : ObservableObject
    {
        private string _name;
        private string _value;
        private XShape _owner;

        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        public string Value
        {
            get { return _value; }
            set { Update(ref _value, value); }
        }

        public XShape Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }
    }
}
