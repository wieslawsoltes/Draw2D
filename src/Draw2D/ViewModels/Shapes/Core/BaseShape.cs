// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public abstract class BaseShape : ViewModelBase, IBaseShape
    {
        internal static IBounds s_bounds = null;
        internal static IShapeDecorator s_decorator = null;

        private string _styleId;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string StyleId
        {
            get => _styleId;
            set => Update(ref _styleId, value);
        }

        [IgnoreDataMember]
        public virtual IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public virtual IShapeDecorator Decorator { get; } = s_decorator;

        public abstract void GetPoints(IList<IPointShape> points);

        public abstract void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r);

        public abstract void Move(ISelectionState selectionState, double dx, double dy);

        public virtual void Select(ISelectionState selectionState)
        {
            if (!selectionState.IsSelected(this))
            {
                selectionState.Select(this);
            }
        }

        public virtual void Deselect(ISelectionState selectionState)
        {
            if (selectionState.IsSelected(this))
            {
                selectionState.Deselect(this);
            }
        }

        public abstract object Copy(Dictionary<object, object> shared);
    }
}
