using System.Collections.Generic;

namespace Draw2D.ViewModels.Containers
{
    public interface ILibrary<T> : IDirty where T : INode, ICopyable, IDirty
    {
        IList<T> Items { get; set; }
        T CurrentItem { get; set; }
        void UpdateCache();
        void Add(T value);
        void Remove(T value);
        T Get(string title);
    }
}
