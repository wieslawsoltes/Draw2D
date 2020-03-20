using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class GroupLibrary : Library<GroupShape>, IGroupLibrary
    {
        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new GroupLibrary()
            {
                Name = this.Name,
                CurrentItem = (GroupShape)this.CurrentItem?.Copy(shared),
                Items = new ObservableCollection<GroupShape>()
            };

            foreach (var item in this.Items)
            {
                if (item is ICopyable copyable)
                {
                    copy.Items.Add((GroupShape)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

    }
}
