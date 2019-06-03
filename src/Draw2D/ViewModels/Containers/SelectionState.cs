// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class SelectionState : ViewModelBase, ISelectionState
    {
        private IBaseShape _hovered;
        private IBaseShape _selected;
        private ISet<IBaseShape> _shapes;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Hovered
        {
            get => _hovered;
            set => Update(ref _hovered, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IBaseShape Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISet<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public void Hover(IBaseShape shape)
        {
            if (shape != null)
            {
                shape.Select(this);
                Hovered = shape;
                this.MarkAsDirty(true);
            }
        }

        public void Dehover()
        {
            if (_hovered != null)
            {
                _hovered.Deselect(this);
                Hovered = null;
                this.MarkAsDirty(true);
            }
        }

        public bool IsSelected(IBaseShape shape)
        {
            if (shape != null && _shapes.Contains(shape))
            {
                return true;
            }
            return false;
        }

        public void Select(IBaseShape shape)
        {
            if (shape != null)
            {
                if (_shapes.Count == 0)
                {
                    Selected = shape;
                }
                _shapes.Add(shape);
                this.MarkAsDirty(true);
            }
        }

        public void Deselect(IBaseShape shape)
        {
            if (shape != null)
            {
                _shapes.Remove(shape);
                if (_shapes.Count == 0)
                {
                    Selected = null;
                }
                this.MarkAsDirty(true);
            }
        }

        public void Clear()
        {
            _shapes.Clear();
            Selected = null;
            this.MarkAsDirty(true);
        }
    }
}
