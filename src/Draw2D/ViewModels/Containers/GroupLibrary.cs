// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class GroupLibrary : ViewModelBase, IGroupLibrary
    {
        private Dictionary<string, GroupShape> _groupLibraryCache;
        private IList<GroupShape> _groups;
        private GroupShape _currentGroup;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<GroupShape> Groups
        {
            get => _groups;
            set => Update(ref _groups, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GroupShape CurrentGroup
        {
            get => _currentGroup;
            set => Update(ref _currentGroup, value);
        }

        public override void Invalidate()
        {
            if (_groups != null)
            {
                foreach (var group in _groups)
                {
                    group.Invalidate();
                }
            }

            _currentGroup?.Invalidate();

            base.Invalidate();
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new GroupLibrary()
            {
                Name = this.Name,
                CurrentGroup = (GroupShape)this.CurrentGroup?.Copy(shared),
                Groups = new ObservableCollection<GroupShape>()
            };

            foreach (var style in this.Groups)
            {
                if (style is ICopyable copyable)
                {
                    copy.Groups.Add((GroupShape)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

        public void UpdateCache()
        {
            if (_groups != null)
            {
                if (_groupLibraryCache == null)
                {
                    _groupLibraryCache = new Dictionary<string, GroupShape>();
                }
                else
                {
                    _groupLibraryCache.Clear();
                }

                foreach (var style in _groups)
                {
                    _groupLibraryCache[style.Title] = style;
                }
            }
        }

        public void Add(GroupShape value)
        {
            if (value != null)
            {
                _groups.Add(value);

                if (_groupLibraryCache == null)
                {
                    _groupLibraryCache = new Dictionary<string, GroupShape>();
                }

                _groupLibraryCache[value.Title] = value;
            }
        }

        public void Remove(GroupShape value)
        {
            if (value != null)
            {
                _groups.Remove(value);

                if (_groupLibraryCache != null)
                {
                    _groupLibraryCache.Remove(value.Title);
                }
            }
        }

        public GroupShape Get(string groupId)
        {
            if (_groupLibraryCache == null)
            {
                UpdateCache();
            }

            if (!_groupLibraryCache.TryGetValue(groupId, out var value))
            {
                foreach (var style in _groups)
                {
                    if (style.Title == groupId)
                    {
                        _groupLibraryCache[style.Title] = style;
                        return style;
                    }
                }
                return null;
            }

            return value;
        }
    }
}
