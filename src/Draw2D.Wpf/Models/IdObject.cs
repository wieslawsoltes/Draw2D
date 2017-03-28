using System;

namespace Draw2D.Models
{
    public abstract class IdObject : BaseObject
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
