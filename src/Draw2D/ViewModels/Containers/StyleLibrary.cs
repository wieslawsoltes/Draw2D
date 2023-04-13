using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers;

[DataContract(IsReference = true)]
public class StyleLibrary : Library<IShapeStyle>, IStyleLibrary
{
    public virtual object Copy(Dictionary<object, object> shared)
    {
        var copy = new StyleLibrary()
        {
            Name = this.Name,
            CurrentItem = (IShapeStyle)this.CurrentItem?.Copy(shared),
            Items = new ObservableCollection<IShapeStyle>()
        };

        foreach (var item in this.Items)
        {
            if (item is ICopyable copyable)
            {
                copy.Items.Add((IShapeStyle)(copyable.Copy(shared)));
            }
        }

        return copy;
    }
}