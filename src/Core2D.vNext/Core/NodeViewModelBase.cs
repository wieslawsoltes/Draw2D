using Core2D.Clipboard;
using Core2D.Factories;
using Core2D.History;
using Core2D.Serializer;

namespace Core2D
{
    public abstract class NodeViewModelBase<T> : ViewModelBase where T : ViewModelBase
    {
        private bool _isExpanded;
        private object _selected;
        private T _owner;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Update(ref _isExpanded, value); }
        }

        public object Selected
        {
            get { return _selected; }
            set { Update(ref _selected, value); }
        }

        public T Owner
        {
            get { return _owner; }
            set { Update(ref _owner, value); }
        }

        public override IVirtualClipboard Clipboard => Owner.Clipboard;

        public override IJsonSerializer Serializer => Owner.Serializer;

        public override IHistory History => Owner.History;

        public override IProjectFactory ProjectFactory => Owner.ProjectFactory;
    }
}
