// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class Library<T> : ViewModelBase, ILibrary<T> where T : INode, ICopyable, IDirty
    {
        private Dictionary<string, T> _itemsCache;
        private IList<T> _items;
        private T _currentItem;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<T> Items
        {
            get => _items;
            set => Update(ref _items, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public T CurrentItem
        {
            get => _currentItem;
            set => Update(ref _currentItem, value);
        }

        public override void Invalidate()
        {
            if (_items != null)
            {
                foreach (var item in _items)
                {
                    item.Invalidate();
                }
            }

            _currentItem?.Invalidate();

            base.Invalidate();
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new Library<T>()
            {
                Name = this.Name,
                CurrentItem = (T)this.CurrentItem?.Copy(shared),
                Items = new ObservableCollection<T>()
            };

            foreach (var item in this.Items)
            {
                if (item is ICopyable copyable)
                {
                    copy.Items.Add((T)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

        public void UpdateCache()
        {
            if (_items != null)
            {
                if (_itemsCache == null)
                {
                    _itemsCache = new Dictionary<string, T>();
                }
                else
                {
                    _itemsCache.Clear();
                }

                foreach (var item in _items)
                {
                    _itemsCache[item.Id] = item;
                }
            }
        }

        public void Add(T value)
        {
            if (value != null)
            {
                _items.Add(value);

                if (_itemsCache == null)
                {
                    _itemsCache = new Dictionary<string, T>();
                }

                _itemsCache[value.Id] = value;
            }
        }

        public void Remove(T value)
        {
            if (value != null)
            {
                _items.Remove(value);

                if (_itemsCache != null)
                {
                    _itemsCache.Remove(value.Id);
                }
            }
        }

        public T Get(string id)
        {
            if (_itemsCache == null)
            {
                UpdateCache();
            }

            if (!_itemsCache.TryGetValue(id, out var value))
            {
                foreach (var item in _items)
                {
                    if (item.Id == id)
                    {
                        _itemsCache[item.Id] = item;
                        return item;
                    }
                }
                return default;
            }

            return value;
        }
    }
}
