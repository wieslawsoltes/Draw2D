using System;

namespace Draw2D.Core
{
    public abstract class IdObject : ObservableObject
    {
        private Guid _id;
        private string _name;

        public Guid Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }
    }
}
