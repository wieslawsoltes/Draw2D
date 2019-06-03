// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public abstract class ConnectableShape : BaseShape, IConnectable
    {
        internal static new IBounds s_bounds = null;
        internal static new IShapeDecorator s_decorator = null;

        private IList<IPointShape> _points;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IPointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        public ConnectableShape()
        {
        }

        public ConnectableShape(IList<IPointShape> points)
        {
            this.Points = points;
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

        public override void GetPoints(IList<IPointShape> points)
        {
            if (_points != null)
            {
                foreach (var point in _points)
                {
                    points.Add(point);
                }
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r)
        {
            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var point in Points)
                {
                    if (renderer.SelectionState?.IsSelected(point) ?? false)
                    {
                        point.Draw(dc, renderer, dx, dy, scale, mode, db, r);
                    }
                }
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            foreach (var point in Points)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);

            foreach (var point in Points)
            {
                point.Select(selectionState);
            }
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);

            foreach (var point in Points)
            {
                point.Deselect(selectionState);
            }
        }

        private bool CanConnect(IPointShape point)
        {
            return _points.Contains(point) == false;
        }

        public virtual bool Connect(IPointShape point, IPointShape target)
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

            for (int i = 0; i < _points.Count; i++)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(ConnectableShape)}: Disconnected from {nameof(Points)} #{i}");
#endif
                _points[i] = (IPointShape)_points[i].Copy(null);
                result = true;
            }

            return result;
        }
    }
}
