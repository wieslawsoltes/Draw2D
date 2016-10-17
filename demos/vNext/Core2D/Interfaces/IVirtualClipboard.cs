using System;
using System.ComponentModel;

namespace Core2D.Clipboard
{
    public interface IVirtualClipboard : INotifyPropertyChanged
    {
        bool HasData { get; }

        Type DataType { get; }

        object Data { get; set; }
    }
}
