using System;

namespace Core2D.Clipboard
{
    public class VirtualClipboard : ObservableObject, IVirtualClipboard
    {
        private object _data;
        public bool _containsData;
        public Type _dataType;

        public object Data
        {
            get { return _data; }
            set
            {
                Update(ref _data, value);
                DataType = _data?.GetType();
                HasData = _data != null;
            }
        }

        public bool HasData
        {
            get { return _containsData; }
            private set { Update(ref _containsData, value); }
        }

        public Type DataType
        {
            get { return _dataType; }
            private set { Update(ref _dataType, value); }
        }
    }
}
