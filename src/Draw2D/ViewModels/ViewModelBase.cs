using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels;

[DataContract(IsReference = true)]
public abstract class ViewModelBase : INode, IDirty, INotifyPropertyChanged
{
    private string _id = null;
    private string _name = null;
    private string _title = null;
    private object _owner = null;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual string Id
    {
        get => _id;
        set => Update(ref _id, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public virtual string Name
    {
        get => _name;
        set => Update(ref _name, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Title
    {
        get => _title;
        set => Update(ref _title, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public object Owner
    {
        get => _owner;
        set => Update(ref _owner, value);
    }

    [IgnoreDataMember]
    public bool IsDirty { get; set; }

    public virtual void MarkAsDirty(bool value) => IsDirty = value;

    public virtual bool IsTreeDirty() => this.IsDirty;

    public virtual void Invalidate()
    {
        if (this.IsDirty)
        {
            this.IsDirty = false;
        }
    }

    public void SetUniqueId()
    {
        Id = Guid.NewGuid().ToString();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void Notify([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (!Equals(field, value))
        {
            field = value;
            IsDirty = true;
            Notify(propertyName);
            return true;
        }
        return false;
    }
}