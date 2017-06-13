// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Draw2D
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        internal bool IsDirty { get; set; }

        public void MarkAsDirty(bool value) => IsDirty = value;

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
}
