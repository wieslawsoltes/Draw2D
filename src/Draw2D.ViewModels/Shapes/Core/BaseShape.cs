// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public abstract class BaseShape : ViewModelBase, IBaseShape, IConnectable
    {
        internal static IBounds s_bounds = null;
        internal static IShapeDecorator s_decorator = null;

        private string _styleId;
        private IList<IPointShape> _points;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string StyleId
        {
            get => _styleId;
            set => Update(ref _styleId, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        [IgnoreDataMember]
        public virtual IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public virtual IShapeDecorator Decorator { get; } = s_decorator;

        public virtual void GetPoints(IList<IPointShape> points)
        {
            if (_points != null)
            {
                foreach (var point in _points)
                {
                    points.Add(point);
                }
            }
        }

        public override void Invalidate()
        {
            if (_points != null)
            {
                foreach (var point in _points)
                {
                    point.Invalidate();
                }
            }

            base.Invalidate();
        }

        public virtual void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (_points != null)
            {
                foreach (var point in _points)
                {
                    point.Draw(dc, renderer, dx, dy, scale, db, r);
                }
            }
        }

        public virtual void Move(ISelectionState selectionState, double dx, double dy)
        {
            foreach (var point in Points)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }
        }

        public virtual void Select(ISelectionState selectionState)
        {
            if (!selectionState.IsSelected(this))
            {
                selectionState.Select(this);
            }

            if (_points != null)
            {
                foreach (var point in _points)
                {
                    point.Select(selectionState);
                }
            }
        }

        public virtual void Deselect(ISelectionState selectionState)
        {
            if (selectionState.IsSelected(this))
            {
                selectionState.Deselect(this);
            }

            if (_points != null)
            {
                foreach (var point in _points)
                {
                    point.Deselect(selectionState);
                }
            }
        }

        private bool CanConnect(IPointShape point)
        {
            return _points?.Contains(point) == false;
        }

        public virtual bool Connect(IPointShape point, IPointShape target)
        {
            if (_points != null)
            {
                if (CanConnect(point))
                {
                    int index = _points.IndexOf(target);
                    if (index >= 0)
                    {
#if DEBUG_CONNECTORS
                        Log.WriteLine($"{nameof(ConnectableShape)} Connected to Points");
#endif
                        _points[index] = point;
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual bool Disconnect(IPointShape point, out IPointShape result)
        {
            result = null;
            return false;
        }

        public virtual bool Disconnect()
        {
            bool result = false;

            if (_points != null)
            {
                for (int i = 0; i < _points.Count; i++)
                {
#if DEBUG_CONNECTORS
                    Log.WriteLine($"{nameof(ConnectableShape)}: Disconnected from {nameof(Points)} #{i}");
#endif
                    _points[i] = (IPointShape)_points[i].Copy(null);
                    result = true;
                }
            }

            return result;
        }

        public abstract object Copy(Dictionary<object, object> shared);
    }
}
