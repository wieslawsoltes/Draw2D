using System;
using Core2D.Clipboard;
using Core2D.Factories;
using Core2D.History;
using Core2D.Serializer;

namespace Core2D
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public abstract IVirtualClipboard Clipboard { get; }

        public abstract IJsonSerializer Serializer { get; }

        public abstract IHistory History { get; }

        public abstract IProjectFactory ProjectFactory { get; }

        public abstract void Dispose();
    }
}
