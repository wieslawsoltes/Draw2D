// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class StyleLibrary : ViewModelBase, IStyleLibrary
    {
        private Dictionary<string, ShapeStyle> _styleLibraryCache;
        private IList<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ShapeStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShapeStyle CurrentStyle
        {
            get => _currentStyle;
            set => Update(ref _currentStyle, value);
        }

        public override void Invalidate()
        {
            if (_styles != null)
            {
                foreach (var style in _styles)
                {
                    style.Invalidate();
                    style.Stroke?.Invalidate();
                    style.Fill?.Invalidate();
                    style.TextStyle?.Invalidate();
                    style.TextStyle?.Stroke?.Invalidate();
                }
            }

            _currentStyle?.Invalidate();

            base.Invalidate();
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new StyleLibrary()
            {
                Name = this.Name,
                CurrentStyle = (ShapeStyle)this.CurrentStyle?.Copy(shared),
                Styles = new ObservableCollection<ShapeStyle>()
            };

            foreach (var style in this.Styles)
            {
                if (style is ICopyable copyable)
                {
                    copy.Styles.Add((ShapeStyle)(copyable.Copy(shared)));
                }
            }

            return copy;
        }

        public void UpdateCache()
        {
            if (_styles != null)
            {
                if (_styleLibraryCache == null)
                {
                    _styleLibraryCache = new Dictionary<string, ShapeStyle>();
                }
                else
                {
                    _styleLibraryCache.Clear();
                }

                foreach (var style in _styles)
                {
                    _styleLibraryCache[style.Title] = style;
                }
            }
        }

        public void Add(ShapeStyle value)
        {
            if (value != null)
            {
                _styles.Add(value);

                if (_styleLibraryCache == null)
                {
                    _styleLibraryCache = new Dictionary<string, ShapeStyle>();
                }

                _styleLibraryCache[value.Title] = value;
            }
        }

        public void Remove(ShapeStyle value)
        {
            if (value != null)
            {
                _styles.Remove(value);

                if (_styleLibraryCache != null)
                {
                    _styleLibraryCache.Remove(value.Title);
                }
            }
        }

        public ShapeStyle Get(string styleId)
        {
            if (_styleLibraryCache == null)
            {
                UpdateCache();
            }

            if (!_styleLibraryCache.TryGetValue(styleId, out var value))
            {
                foreach (var style in _styles)
                {
                    if (style.Title == styleId)
                    {
                        _styleLibraryCache[style.Title] = style;
                        return style;
                    }
                }
                return null;
            }

            return value;
        }
    }
}
