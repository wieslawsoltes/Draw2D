// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public class TextShape : BaseShape
    {
        internal static new IBounds s_bounds = new TextBounds();
        internal static new IShapeDecorator s_decorator = new TextDecorator();

        private IPointShape _topLeft;
        private IPointShape _bottomRight;
        private Text _text;

        [IgnoreDataMember]
        public override IBounds Bounds { get; } = s_bounds;

        [IgnoreDataMember]
        public override IShapeDecorator Decorator { get; } = s_decorator;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public TextShape()
        {
        }

        public TextShape(IPointShape topLeft, IPointShape bottomRight)
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public TextShape(Text text, IPointShape topLeft, IPointShape bottomRight)
        {
            this.Text = text;
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(TopLeft);
            points.Add(BottomRight);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            _topLeft?.Invalidate();

            _bottomRight?.Invalidate();

            _text?.Invalidate();

            base.Invalidate();
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
        {
            if (StyleId != null)
            {
                renderer.DrawText(dc, this, StyleId, dx, dy, scale);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_topLeft))
            {
                _topLeft.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_bottomRight))
            {
                _bottomRight.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            TopLeft.Select(selectionState);
            BottomRight.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            TopLeft.Deselect(selectionState);
            BottomRight.Deselect(selectionState);
        }

        private bool CanConnect(IPointShape point)
        {
            return TopLeft != point
                && BottomRight != point;
        }

        public override bool Connect(IPointShape point, IPointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (TopLeft == target)
                {
#if DEBUG_CONNECTORS
                    Log.WriteLine($"{nameof(TextShape)}: Connected to {nameof(TopLeft)}");
#endif
                    this.TopLeft = point;
                    return true;
                }
                else if (BottomRight == target)
                {
#if DEBUG_CONNECTORS
                    Log.WriteLine($"{nameof(TextShape)}: Connected to {nameof(BottomRight)}");
#endif
                    this.BottomRight = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(IPointShape point, out IPointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (TopLeft == point)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(TopLeft)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.TopLeft = result;
                return true;
            }
            else if (BottomRight == point)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(BottomRight)}");
#endif
                result = (IPointShape)(point.Copy(null));
                this.BottomRight = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.TopLeft != null)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(TopLeft)}");
#endif
                this.TopLeft = (IPointShape)(this.TopLeft.Copy(null));
                result = true;
            }

            if (this.BottomRight != null)
            {
#if DEBUG_CONNECTORS
                Log.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(BottomRight)}");
#endif
                this.BottomRight = (IPointShape)(this.BottomRight.Copy(null));
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new TextShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StyleId = this.StyleId,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (IPointShape)shared[this.TopLeft];
                copy.BottomRight = (IPointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((IPointShape)shared[point]);
                }

                shared[this] = copy;
                shared[copy] = this;
            }

            return copy;
        }
    }
}
