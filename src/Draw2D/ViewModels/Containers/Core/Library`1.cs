// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public abstract class Library<T> : ViewModelBase, ILibrary<T> where T : INode, ICopyable, IDirty
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
                    _itemsCache[item.Title] = item;
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

                _itemsCache[value.Title] = value;
            }
        }

        public void Remove(T value)
        {
            if (value != null)
            {
                _items.Remove(value);

                if (_itemsCache != null)
                {
                    _itemsCache.Remove(value.Title);
                }
            }
        }

        public T Get(string title)
        {
            if (_itemsCache == null)
            {
                UpdateCache();
            }

            if (!_itemsCache.TryGetValue(title, out var value))
            {
                foreach (var item in _items)
                {
                    if (item.Title == title)
                    {
                        _itemsCache[item.Title] = item;
                        return item;
                    }
                }
                return default;
            }

            return value;
        }
    }
}
