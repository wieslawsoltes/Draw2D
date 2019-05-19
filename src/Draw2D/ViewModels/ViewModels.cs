// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

namespace Draw2D.ViewModels
{
    public static class ForEachExtension
    {
        public static void ForEach<T>(this IList<T> list, Action<T> action)
        {
            foreach (var t in list)
            {
                action(t);
            }
        }
    }

    [Flags]
    public enum DrawMode
    {
        None = 0,
        Point = 1,
        Shape = 2,
        All = Point | Shape
    }

    public enum EditMode
    {
        Touch,
        Mouse
    }

    [Flags]
    public enum Modifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4
    }

    public interface IInputTarget
    {
        void LeftDown(double x, double y, Modifier modifier);
        void LeftUp(double x, double y, Modifier modifier);
        void RightDown(double x, double y, Modifier modifier);
        void RightUp(double x, double y, Modifier modifier);
        void Move(double x, double y, Modifier modifier);
        double GetWidth();
        double GetHeight();
    }

    public interface IInputService
    {
        Action Capture { get; set; }
        Action Release { get; set; }
        Func<bool> IsCaptured { get; set; }
        Action Redraw { get; set; }
    }

    public enum FitMode
    {
        None,
        Reset,
        Center,
        Fill,
        Uniform,
        UniformToFill
    }

    public interface IZoomServiceState : IDirty, ICopyable
    {
        double ZoomSpeed { get; set; }
        double ZoomX { get; set; }
        double ZoomY { get; set; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        bool IsPanning { get; set; }
        bool IsZooming { get; set; }
        bool CustomDraw { get; set; }
        FitMode InitFitMode { get; set; }
        FitMode AutoFitMode { get; set; }
    }

    public interface IZoomService
    {
        IZoomServiceState ZoomServiceState { get; set; }
        void Wheel(double delta, double x, double y);
        void Pressed(double x, double y);
        void Released(double x, double y);
        void Moved(double x, double y);
        void Invalidate(bool redraw);
        void ZoomTo(double zoom, double x, double y);
        void ZoomDeltaTo(double delta, double x, double y);
        void StartPan(double x, double y);
        void PanTo(double x, double y);
        void Reset();
        void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void ResetZoom(bool redraw);
        void CenterZoom(bool redraw);
        void FillZoom(bool redraw);
        void UniformZoom(bool redraw);
        void UniformToFillZoom(bool redraw);
    }

    public interface IDrawTarget
    {
        IInputService InputService { get; set; }
        IZoomService ZoomService { get; set; }
        void Draw(object context, double width, double height, double dx, double dy, double zx, double zy);
    }

    public interface IConnectable
    {
        bool Connect(PointShape point, PointShape target);
        bool Disconnect(PointShape point, out PointShape result);
        bool Disconnect();
    }

    public interface ICopyable
    {
        object Copy(Dictionary<object, object> shared);
    }

    public interface IDrawable
    {
        ShapeStyle Style { get; set; }
        void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r);
    }

    public interface IDirty
    {
        bool IsDirty { get; set; }
        void MarkAsDirty(bool isDirty);
        void Invalidate();
    }

    public interface ISelectionState : IDirty, ICopyable
    {
        BaseShape Hovered { get; set; }
        BaseShape Selected { get; set; }
        ISet<BaseShape> Shapes { get; set; }
        void Hover(BaseShape shape);
        void Dehover();
        bool IsSelected(BaseShape shape);
        void Select(BaseShape shape);
        void Deselect(BaseShape shape);
        void Clear();
    }

    public interface ISelection : IDirty
    {
        void Cut(IToolContext context);
        void Copy(IToolContext context);
        void Paste(IToolContext context);
        void Delete(IToolContext context);
        void Group(IToolContext context);
        void SelectAll(IToolContext context);
        void Connect(IToolContext context, PointShape point);
        void Disconnect(IToolContext context, PointShape point);
        void Disconnect(IToolContext context, BaseShape shape);
    }

    public interface ISelectable
    {
        void Move(ISelectionState selectionState, double dx, double dy);
        void Select(ISelectionState selectionState);
        void Deselect(ISelectionState selectionState);
    }

    public interface IShapeRenderer : IDisposable
    {
        double Scale { get; set; }
        ISelectionState SelectionState { get; set; }
        void DrawLine(object dc, LineShape line, ShapeStyle style, double dx, double dy);
        void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, ShapeStyle style, double dx, double dy);
        void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, ShapeStyle style, double dx, double dy);
        void DrawConic(object dc, ConicShape conic, ShapeStyle style, double dx, double dy);
        void DrawPath(object dc, PathShape path, ShapeStyle style, double dx, double dy);
        void DrawRectangle(object dc, RectangleShape rectangle, ShapeStyle style, double dx, double dy);
        void DrawEllipse(object dc, EllipseShape ellipse, ShapeStyle style, double dx, double dy);
        void DrawText(object dc, TextShape text, ShapeStyle style, double dx, double dy);
    }

    public interface IShapeDecorator
    {
        void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode);
    }

    public interface IHitTest
    {
        Dictionary<string, IBounds> Registered { get; set; }
        void Register(IBounds hitTest);
        PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, PointShape exclude);
        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius);
        BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius);
        ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius);
    }

    public interface ITool
    {
        string Title { get; }
        IList<PointIntersection> Intersections { get; set; }
        IList<PointFilter> Filters { get; set; }
        void LeftDown(IToolContext context, double x, double y, Modifier modifier);
        void LeftUp(IToolContext context, double x, double y, Modifier modifier);
        void RightDown(IToolContext context, double x, double y, Modifier modifier);
        void RightUp(IToolContext context, double x, double y, Modifier modifier);
        void Move(IToolContext context, double x, double y, Modifier modifier);
        void Clean(IToolContext context);
    }

    public interface IBounds
    {
        string TargetType { get; }
        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest);
        BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest);
        BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest);
    }

    [DataContract(IsReference = true)]
    public abstract class ViewModelBase : INotifyPropertyChanged, IDirty
    {
        private string _id = null;
        private string _name = null;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public virtual string Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public virtual string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        [IgnoreDataMember]
        public bool IsDirty { get; set; }

        public void MarkAsDirty(bool value) => IsDirty = value;

        public abstract void Invalidate();

        public void SetUniqueId()
        {
            Id = Guid.NewGuid().ToString();
        }

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

    [DataContract(IsReference = true)]
    public class Text : ViewModelBase, ICopyable
    {
        private string _value;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Value
        {
            get => _value;
            set => Update(ref _value, value);
        }

        public Text()
        {
        }

        public Text(string value)
        {
            _value = value;
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Text()
            {
                Value = this.Value
            };
        }
    }

    [DataContract(IsReference = true)]
    public abstract class Settings : ViewModelBase
    {
    }

    [DataContract(IsReference = true)]
    public abstract class PointFilter : ViewModelBase
    {
        private IList<BaseShape> _guides;

        public abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<BaseShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(guide);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(guide);
            }
            Guides.Clear();
        }
    }

    [DataContract(IsReference = true)]
    public abstract class PointIntersection : ViewModelBase
    {
        private IList<PointShape> _intersections;

        public abstract string Title { get; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointShape> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        public abstract void Find(IToolContext context, BaseShape shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(point);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(point);
            }
            Intersections.Clear();
        }
    }
}

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class ArgbColor : ViewModelBase, ICopyable
    {
        private byte _a;
        private byte _r;
        private byte _g;
        private byte _b;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte A
        {
            get => _a;
            set => Update(ref _a, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte R
        {
            get => _r;
            set => Update(ref _r, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte G
        {
            get => _g;
            set => Update(ref _g, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public byte B
        {
            get => _b;
            set => Update(ref _b, value);
        }

        public ArgbColor()
        {
        }

        public ArgbColor(byte a, byte r, byte g, byte b)
        {
            this.A = a;
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ArgbColor()
            {
                A = this.A,
                R = this.R,
                G = this.G,
                B = this.B
            };
        }
    }

    public enum HAlign
    {
        Left,
        Center,
        Right
    }

    public enum VAlign
    {
        Top,
        Center,
        Bottom
    }

    [DataContract(IsReference = true)]
    public class TextStyle : ViewModelBase, ICopyable
    {
        private string _fontFamily;
        private double _fontSize;
        private HAlign _hAlign;
        private VAlign _vAlign;
        private ArgbColor _stroke;
        private bool _isStroked;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string FontFamily
        {
            get => _fontFamily;
            set => Update(ref _fontFamily, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double FontSize
        {
            get => _fontSize;
            set => Update(ref _fontSize, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public HAlign HAlign
        {
            get => _hAlign;
            set => Update(ref _hAlign, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public VAlign VAlign
        {
            get => _vAlign;
            set => Update(ref _vAlign, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        public TextStyle()
        {
        }

        public TextStyle(string fontFamily, double fontSize, HAlign hAlign, VAlign vAlign, ArgbColor stroke, bool isStroked)
        {
            this.FontFamily = fontFamily;
            this.FontSize = fontSize;
            this.HAlign = hAlign;
            this.VAlign = vAlign;
            this.Stroke = stroke;
            this.IsStroked = isStroked;
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new TextStyle()
            {
                Name = this.Name,
                FontFamily = this.FontFamily,
                FontSize = this.FontSize,
                HAlign = this.HAlign,
                VAlign = this.VAlign,
                Stroke = (ArgbColor)this.Stroke.Copy(shared),
                IsStroked = this.IsStroked
            };
        }
    }

    [DataContract(IsReference = true)]
    public class ShapeStyle : ViewModelBase, ICopyable
    {
        private string _title;
        private ArgbColor _stroke;
        private ArgbColor _fill;
        private double _thickness;
        private bool _isStroked;
        private bool _isFilled;
        private TextStyle _textStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Stroke
        {
            get => _stroke;
            set => Update(ref _stroke, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Fill
        {
            get => _fill;
            set => Update(ref _fill, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Thickness
        {
            get => _thickness;
            set => Update(ref _thickness, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TextStyle TextStyle
        {
            get => _textStyle;
            set => Update(ref _textStyle, value);
        }

        public ShapeStyle()
        {
        }

        public ShapeStyle(string title, ArgbColor stroke, ArgbColor fill, double thickness, bool isStroked, bool isFilled, TextStyle textStyle)
        {
            this.Title = title;
            this.Stroke = stroke;
            this.Fill = fill;
            this.Thickness = thickness;
            this.IsStroked = isStroked;
            this.IsFilled = isFilled;
            this.TextStyle = textStyle;
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Title = this.Title,
                Stroke = (ArgbColor)this.Stroke.Copy(shared),
                Fill = (ArgbColor)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
                TextStyle = (TextStyle)this.TextStyle.Copy(shared)
            };
        }
    }
}

namespace Draw2D.ViewModels.Shapes
{
    [DataContract(IsReference = true)]
    public abstract class BaseShape : ViewModelBase, IDrawable, ISelectable, ICopyable
    {
        private ShapeStyle _style;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShapeStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        public abstract void GetPoints(IList<PointShape> points);

        public abstract void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r);

        public override void Invalidate()
        {
            _style?.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

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

    [DataContract(IsReference = true)]
    public abstract class BoxShape : ConnectableShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        public BoxShape()
            : base()
        {
        }

        public BoxShape(PointShape topLeft, PointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public override void GetPoints(IList<PointShape> points)
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
            base.Invalidate();

            _topLeft?.Invalidate();

            _bottomRight?.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
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

        private bool CanConnect(PointShape point)
        {
            return TopLeft != point
                && BottomRight != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (TopLeft == target)
                {
                    Debug.WriteLine($"{nameof(BoxShape)}: Connected to {nameof(TopLeft)}");
                    this.TopLeft = point;
                    return true;
                }
                else if (BottomRight == target)
                {
                    Debug.WriteLine($"{nameof(BoxShape)}: Connected to {nameof(BottomRight)}");
                    this.BottomRight = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (TopLeft == point)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(TopLeft)}");
                result = (PointShape)point.Copy(null);
                this.TopLeft = result;
                return true;
            }
            else if (BottomRight == point)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(BottomRight)}");
                result = (PointShape)point.Copy(null);
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
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(TopLeft)}");
                this.TopLeft = (PointShape)this.TopLeft.Copy(null);
                result = true;
            }

            if (this.BottomRight != null)
            {
                Debug.WriteLine($"{nameof(BoxShape)}: Disconnected from {nameof(BottomRight)}");
                this.BottomRight = (PointShape)this.BottomRight.Copy(null);
                result = true;
            }

            return result;
        }
    }

    [DataContract(IsReference = true)]
    public abstract class ConnectableShape : BaseShape, IConnectable
    {
        private IList<PointShape> _points;
        private Text _text;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointShape> Points
        {
            get => _points;
            set => Update(ref _points, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Text Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public ConnectableShape()
        {
        }

        public ConnectableShape(IList<PointShape> points)
        {
            this.Points = points;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var point in Points)
                {
                    if (renderer.SelectionState.IsSelected(point))
                    {
                        point.Draw(dc, renderer, dx, dy, mode, db, r);
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

        private bool CanConnect(PointShape point)
        {
            return _points.Contains(point) == false;
        }

        public virtual bool Connect(PointShape point, PointShape target)
        {
            if (CanConnect(point))
            {
                int index = _points.IndexOf(target);
                if (index >= 0)
                {
                    Debug.WriteLine($"ConnectableShape Connected to Points");
                    _points[index] = point;
                    return true;
                }
            }
            return false;
        }

        public virtual bool Disconnect(PointShape point, out PointShape result)
        {
            result = null;
            return false;
        }

        public virtual bool Disconnect()
        {
            bool result = false;

            for (int i = 0; i < _points.Count; i++)
            {
                Debug.WriteLine($"{nameof(ConnectableShape)}: Disconnected from {nameof(Points)} #{i}");
                _points[i] = (PointShape)_points[i].Copy(null);
                result = true;
            }

            return result;
        }
    }

    [DataContract(IsReference = true)]
    public class ConicShape : ConnectableShape, ICopyable
    {
        private PointShape _startPoint;
        private PointShape _point1;
        private PointShape _point2;
        private double _weight;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Weight
        {
            get => _weight;
            set => Update(ref _weight, value);
        }

        public ConicShape()
            : base()
        {
        }

        public ConicShape(PointShape startPoint, PointShape point1, PointShape point2, double weight)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
            this.Weight = weight;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point1);
            points.Add(Point2);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _startPoint?.Invalidate();

            _point1?.Invalidate();

            _point2?.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawConic(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(_startPoint))
                {
                    _startPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point1))
                {
                    _point1.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point2))
                {
                    _point2.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point1))
            {
                _point1.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point2))
            {
                _point2.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point1.Select(selectionState);
            Point2.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point1.Deselect(selectionState);
            Point2.Deselect(selectionState);
        }

        private bool CanConnect(PointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(StartPoint)}");
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point1)}");
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point2)}");
                    this.Point2 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
                result = (PointShape)point.Copy(null);
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
                result = (PointShape)point.Copy(null);
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
                result = (PointShape)point.Copy(null);
                this.Point2 = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
                this.StartPoint = (PointShape)this.StartPoint.Copy(null);
                result = true;
            }

            if (this.Point1 != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
                this.Point1 = (PointShape)this.Point1.Copy(null);
                result = true;
            }

            if (this.Point2 != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
                this.Point2 = (PointShape)this.Point2.Copy(null);
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new ConicShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared),
                Weight = this.Weight
            };

            if (shared != null)
            {
                copy.StartPoint = (PointShape)shared[this.StartPoint];
                copy.Point1 = (PointShape)shared[this.Point1];
                copy.Point2 = (PointShape)shared[this.Point2];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class CubicBezierShape : ConnectableShape, ICopyable
    {
        private PointShape _startPoint;
        private PointShape _point1;
        private PointShape _point2;
        private PointShape _point3;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        public CubicBezierShape()
            : base()
        {
        }

        public CubicBezierShape(PointShape startPoint, PointShape point1, PointShape point2, PointShape point3)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
            this.Point3 = point3;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point1);
            points.Add(Point2);
            points.Add(Point3);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _startPoint?.Invalidate();

            _point1?.Invalidate();

            _point2?.Invalidate();

            _point3?.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawCubicBezier(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(_startPoint))
                {
                    _startPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point1))
                {
                    _point1.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point2))
                {
                    _point2.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point3))
                {
                    _point3.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point1))
            {
                _point1.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point2))
            {
                _point2.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point3))
            {
                _point3.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point1.Select(selectionState);
            Point2.Select(selectionState);
            Point3.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point1.Deselect(selectionState);
            Point2.Deselect(selectionState);
            Point3.Deselect(selectionState);
        }

        private bool CanConnect(PointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point
                && Point3 != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
                    Debug.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(StartPoint)}");
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
                    Debug.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(Point1)}");
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
                    Debug.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(Point2)}");
                    this.Point2 = point;
                    return true;
                }
                else if (Point3 == target)
                {
                    Debug.WriteLine($"{nameof(CubicBezierShape)}: Connected to {nameof(Point3)}");
                    this.Point3 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(StartPoint)}");
                result = (PointShape)point.Copy(null);
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point1)}");
                result = (PointShape)point.Copy(null);
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point2)}");
                result = (PointShape)point.Copy(null);
                this.Point2 = result;
                return true;
            }
            else if (Point3 == point)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point3)}");
                result = (PointShape)point.Copy(null);
                this.Point3 = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(StartPoint)}");
                this.StartPoint = (PointShape)this.StartPoint.Copy(null);
                result = true;
            }

            if (this.Point1 != null)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point1)}");
                this.Point1 = (PointShape)this.Point1.Copy(null);
                result = true;
            }

            if (this.Point2 != null)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point2)}");
                this.Point2 = (PointShape)this.Point2.Copy(null);
                result = true;
            }

            if (this.Point3 != null)
            {
                Debug.WriteLine($"{nameof(CubicBezierShape)}: Disconnected from {nameof(Point3)}");
                this.Point3 = (PointShape)this.Point3.Copy(null);
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new CubicBezierShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (PointShape)shared[this.StartPoint];
                copy.Point1 = (PointShape)shared[this.Point1];
                copy.Point2 = (PointShape)shared[this.Point2];
                copy.Point3 = (PointShape)shared[this.Point3];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseShape : BoxShape, ICopyable
    {
        public EllipseShape()
            : base()
        {
        }

        public EllipseShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override void Invalidate()
        {
            base.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawEllipse(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(TopLeft))
                {
                    TopLeft.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(BottomRight))
                {
                    BottomRight.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new EllipseShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    public static class EllipseShapeExtensions
    {
        public static Rect2 ToRect2(this EllipseShape ellipse, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                ellipse.TopLeft.X, ellipse.TopLeft.Y,
                ellipse.BottomRight.X, ellipse.BottomRight.Y,
                dx, dy);
        }

        public static EllipseShape FromRect2(this Rect2 rect)
        {
            return new EllipseShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2())
            {
                Points = new ObservableCollection<PointShape>()
            };
        }
    }

    [DataContract(IsReference = true)]
    public class FigureShape : BaseShape, ICanvasContainer, ICopyable
    {
        private IList<BaseShape> _shapes;
        private bool _isFilled;
        private bool _isClosed;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        public FigureShape()
            : base()
        {
        }

        public FigureShape(IList<BaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public FigureShape(string name)
            : this()
        {
            this.Name = name;
        }

        public FigureShape(string name, IList<BaseShape> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            foreach (var shape in Shapes)
            {
                shape.GetPoints(points);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, mode, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            var points = new List<PointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new FigureShape()
            {
                Shapes = new ObservableCollection<BaseShape>(),
                Name = this.Name,
                Style = this.Style,
                IsFilled = this.IsFilled,
                IsClosed = this.IsClosed
            };

            foreach (var shape in this.Shapes)
            {
                if (shape is ICopyable copyable)
                {
                    copy.Shapes.Add((BaseShape)copyable.Copy(shared));
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class GroupShape : ConnectableShape, ICopyable
    {
        private string _title;
        private IList<BaseShape> _shapes;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public GroupShape()
            : base()
        {
        }

        public GroupShape(IList<BaseShape> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public GroupShape(string title)
            : this()
        {
            this.Title = title;
        }

        public GroupShape(string title, IList<BaseShape> shapes)
            : base()
        {
            this.Title = title;
            this.Shapes = shapes;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            foreach (var point in Points)
            {
                points.Add(point);
            }

            foreach (var shape in Shapes)
            {
                shape.GetPoints(points);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var point in Points)
            {
                point.Invalidate();
            }

            foreach (var shape in Shapes)
            {
                shape.Invalidate();
            }

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, mode, db, r);
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            var points = new List<PointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }

            base.Move(selectionState, dx, dy);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new GroupShape()
            {
                Name = this.Name,
                Title = this.Title,
                Points = new ObservableCollection<PointShape>(),
                Shapes = new ObservableCollection<BaseShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }

                foreach (var shape in this.Shapes)
                {
                    if (shape is ICopyable copyable)
                    {
                        copy.Shapes.Add((BaseShape)copyable.Copy(shared));
                    }
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class LineShape : ConnectableShape, ICopyable
    {
        private PointShape _startPoint;
        private PointShape _point;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        public LineShape()
            : base()
        {
        }

        public LineShape(PointShape startPoint, PointShape point)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point = point;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _startPoint?.Invalidate();

            _point?.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawLine(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(_startPoint))
                {
                    _startPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point))
                {
                    _point.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point))
            {
                _point.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point.Deselect(selectionState);
        }

        private bool CanConnect(PointShape point)
        {
            return StartPoint != point
                && Point != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
                    Debug.WriteLine($"{nameof(LineShape)}: Connected to {nameof(StartPoint)}");
                    this.StartPoint = point;
                    return true;
                }
                else if (Point == target)
                {
                    Debug.WriteLine($"{nameof(LineShape)}: Connected to {nameof(Point)}");
                    this.Point = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
                Debug.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(StartPoint)}");
                result = (PointShape)point.Copy(null);
                this.StartPoint = result;
                return true;
            }
            else if (Point == point)
            {
                Debug.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(Point)}");
                result = (PointShape)point.Copy(null);
                this.Point = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
                Debug.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(StartPoint)}");
                this.StartPoint = (PointShape)this.StartPoint.Copy(null);
                result = true;
            }

            if (this.Point != null)
            {
                Debug.WriteLine($"{nameof(LineShape)}: Disconnected from {nameof(Point)}");
                this.Point = (PointShape)this.Point.Copy(null);
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (PointShape)shared[this.StartPoint];
                copy.Point = (PointShape)shared[this.Point];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    public static class LineShapeExtensions
    {
        public static Line2 ToLine2(this LineShape line, double dx = 0.0, double dy = 0.0)
        {
            return Line2.FromPoints(
                line.StartPoint.X, line.StartPoint.Y,
                line.Point.X, line.Point.Y,
                dx, dy);
        }

        public static LineShape FromLine2(this Line2 line)
        {
            return new LineShape(line.A.FromPoint2(), line.B.FromPoint2())
            {
                Points = new ObservableCollection<PointShape>()
            };
        }
    }

    public enum PathFillRule
    {
        EvenOdd,
        Nonzero
    }

    [DataContract(IsReference = true)]
    public class PathShape : ConnectableShape, ICopyable
    {
        private string _title;
        private IList<FigureShape> _figures;
        private PathFillRule _fillRule;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<FigureShape> Figures
        {
            get => _figures;
            set => Update(ref _figures, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        public PathShape()
            : base()
        {
        }

        public PathShape(IList<FigureShape> figures)
            : base()
        {
            this.Figures = figures;
        }

        public PathShape(string title)
            : this()
        {
            this.Title = title;
        }

        public PathShape(string title, IList<FigureShape> figures)
            : base()
        {
            this.Title = title;
            this.Figures = figures;
        }

        public PointShape GetFirstPoint()
        {
            if (_figures.Count > 0)
            {
                var lastFigure = _figures[_figures.Count - 1];
                var shapes = lastFigure.Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[0])
                    {
                        case LineShape line:
                            return line.StartPoint;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.StartPoint;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.StartPoint;
                        case ConicShape conic:
                            return conic.StartPoint;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public PointShape GetLastPoint()
        {
            if (_figures.Count > 0)
            {
                var lastFigure = _figures[_figures.Count - 1];
                var shapes = lastFigure.Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[shapes.Count - 1])
                    {
                        case LineShape line:
                            return line.Point;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.Point3;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.Point2;
                        case ConicShape conic:
                            return conic.Point2;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            foreach (var point in Points)
            {
                points.Add(point);
            }

            foreach (var figure in Figures)
            {
                figure.GetPoints(points);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            foreach (var figure in Figures)
            {
                figure.Invalidate();
            }

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            var isPathSelected = renderer.SelectionState.IsSelected(this);

            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawPath(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                foreach (var figure in Figures)
                {
                    DrawPoints(dc, renderer, dx, dy, mode, db, r, figure, isPathSelected);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        private void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r, FigureShape figure, bool isPathSelected)
        {
            foreach (var shape in figure.Shapes)
            {
                switch (shape)
                {
                    case LineShape line:
                        {
                            var isSelected = renderer.SelectionState.IsSelected(line);

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(line.StartPoint))
                            {
                                line.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(line.Point))
                            {
                                line.Point.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in line.Points)
                            {
                                if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                    case CubicBezierShape cubic:
                        {
                            var isSelected = renderer.SelectionState.IsSelected(cubic);

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(cubic.StartPoint))
                            {
                                cubic.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(cubic.Point1))
                            {
                                cubic.Point1.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(cubic.Point2))
                            {
                                cubic.Point2.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(cubic.Point3))
                            {
                                cubic.Point3.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in cubic.Points)
                            {
                                if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                    case QuadraticBezierShape quadratic:
                        {
                            var isSelected = renderer.SelectionState.IsSelected(quadratic);

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(quadratic.StartPoint))
                            {
                                quadratic.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(quadratic.Point1))
                            {
                                quadratic.Point1.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(quadratic.Point2))
                            {
                                quadratic.Point2.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in quadratic.Points)
                            {
                                if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                    case ConicShape conic:
                        {
                            var isSelected = renderer.SelectionState.IsSelected(conic);

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(conic.StartPoint))
                            {
                                conic.StartPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(conic.Point1))
                            {
                                conic.Point1.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(conic.Point2))
                            {
                                conic.Point2.Draw(dc, renderer, dx, dy, mode, db, r);
                            }

                            foreach (var point in conic.Points)
                            {
                                if (isPathSelected || isSelected || renderer.SelectionState.IsSelected(point))
                                {
                                    point.Draw(dc, renderer, dx, dy, mode, db, r);
                                }
                            }
                        }
                        break;
                }
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            var points = new List<PointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);

            var points = new List<PointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                point.Select(selectionState);
            }
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);

            var points = new List<PointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                point.Deselect(selectionState);
            }
        }

        public bool Validate(bool removeEmptyFigures)
        {
            if (_figures.Count > 0 && _figures[0].Shapes.Count > 0)
            {
                var figures = _figures.ToList();

                if (removeEmptyFigures == true)
                {
                    foreach (var figure in figures)
                    {
                        if (figure.Shapes.Count <= 0)
                        {
                            _figures.Remove(figure);
                            this.MarkAsDirty(true);
                        }
                    }
                }

                if (_figures.Count > 0 && _figures[0].Shapes.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new PathShape()
            {
                Name = this.Name,
                Title = this.Title,
                Points = new ObservableCollection<PointShape>(),
                Figures = new ObservableCollection<FigureShape>(),
                Style = this.Style,
                FillRule = this.FillRule,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                foreach (var figure in this.Figures)
                {
                    copy.Figures.Add((FigureShape)figure.Copy(shared));
                }

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class PointShape : BaseShape, ICopyable
    {
        private double _x;
        private double _y;
        private BaseShape _template;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BaseShape Template
        {
            get => _template;
            set => Update(ref _template, value);
        }

        public PointShape()
        {
        }

        public PointShape(double x, double y, BaseShape template)
        {
            this.X = x;
            this.Y = y;
            this.Template = template;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(this);
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _template?.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (_template != null)
            {
                double offsetX = X;
                double offsetY = Y;
                _template.Draw(dc, renderer, dx + offsetX, dy + offsetY, DrawMode.Shape, db, r);
            }
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new PointShape()
            {
                Style = this.Style,
                X = this.X,
                Y = this.Y,
                Template = this.Template
            };
        }
    }

    public static class PointShapeExtensions
    {
        public static Point2 ToPoint2(this PointShape point)
        {
            return new Point2(point.X, point.Y);
        }

        public static PointShape FromPoint2(this Point2 point, BaseShape template = null)
        {
            return new PointShape(point.X, point.Y, template);
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierShape : ConnectableShape, ICopyable
    {
        private PointShape _startPoint;
        private PointShape _point1;
        private PointShape _point2;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        public QuadraticBezierShape()
            : base()
        {
        }

        public QuadraticBezierShape(PointShape startPoint, PointShape point1, PointShape point2)
            : base()
        {
            this.StartPoint = startPoint;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(StartPoint);
            points.Add(Point1);
            points.Add(Point2);
            foreach (var point in Points)
            {
                points.Add(point);
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _startPoint?.Invalidate();

            _point1?.Invalidate();

            _point2?.Invalidate();


            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawQuadraticBezier(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(_startPoint))
                {
                    _startPoint.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point1))
                {
                    _point1.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(_point2))
                {
                    _point2.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            if (!selectionState.IsSelected(_startPoint))
            {
                _startPoint.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point1))
            {
                _point1.Move(selectionState, dx, dy);
            }

            if (!selectionState.IsSelected(_point2))
            {
                _point2.Move(selectionState, dx, dy);
            }

            base.Move(selectionState, dx, dy);
        }

        public override void Select(ISelectionState selectionState)
        {
            base.Select(selectionState);
            StartPoint.Select(selectionState);
            Point1.Select(selectionState);
            Point2.Select(selectionState);
        }

        public override void Deselect(ISelectionState selectionState)
        {
            base.Deselect(selectionState);
            StartPoint.Deselect(selectionState);
            Point1.Deselect(selectionState);
            Point2.Deselect(selectionState);
        }

        private bool CanConnect(PointShape point)
        {
            return StartPoint != point
                && Point1 != point
                && Point2 != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (StartPoint == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(StartPoint)}");
                    this.StartPoint = point;
                    return true;
                }
                else if (Point1 == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point1)}");
                    this.Point1 = point;
                    return true;
                }
                else if (Point2 == target)
                {
                    Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Connected to {nameof(Point2)}");
                    this.Point2 = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (StartPoint == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
                result = (PointShape)point.Copy(null);
                this.StartPoint = result;
                return true;
            }
            else if (Point1 == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
                result = (PointShape)point.Copy(null);
                this.Point1 = result;
                return true;
            }
            else if (Point2 == point)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
                result = (PointShape)point.Copy(null);
                this.Point2 = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.StartPoint != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(StartPoint)}");
                this.StartPoint = (PointShape)this.StartPoint.Copy(null);
                result = true;
            }

            if (this.Point1 != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point1)}");
                this.Point1 = (PointShape)this.Point1.Copy(null);
                result = true;
            }

            if (this.Point2 != null)
            {
                Debug.WriteLine($"{nameof(QuadraticBezierShape)}: Disconnected from {nameof(Point2)}");
                this.Point2 = (PointShape)this.Point2.Copy(null);
                result = true;
            }

            return result;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new QuadraticBezierShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.StartPoint = (PointShape)shared[this.StartPoint];
                copy.Point1 = (PointShape)shared[this.Point1];
                copy.Point2 = (PointShape)shared[this.Point2];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleShape : BoxShape, ICopyable
    {
        public RectangleShape()
            : base()
        {
        }

        public RectangleShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public override void Invalidate()
        {
            base.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawRectangle(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(TopLeft))
                {
                    TopLeft.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(BottomRight))
                {
                    BottomRight.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new RectangleShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }

    public static class RectangleShapeExtensions
    {
        public static Rect2 ToRect2(this RectangleShape rectangle, double dx = 0.0, double dy = 0.0)
        {
            return Rect2.FromPoints(
                rectangle.TopLeft.X, rectangle.TopLeft.Y,
                rectangle.BottomRight.X, rectangle.BottomRight.Y,
                dx, dy);
        }

        public static RectangleShape FromRect2(this Rect2 rect)
        {
            return new RectangleShape(rect.TopLeft.FromPoint2(), rect.BottomRight.FromPoint2())
            {
                Points = new ObservableCollection<PointShape>()
            };
        }
    }

    [DataContract(IsReference = true)]
    public class TextShape : BoxShape, ICopyable
    {
        public TextShape()
            : base()
        {
        }

        public TextShape(PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
        }

        public TextShape(Text text, PointShape topLeft, PointShape bottomRight)
            : base(topLeft, bottomRight)
        {
            this.Text = text;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Style != null && mode.HasFlag(DrawMode.Shape))
            {
                renderer.DrawText(dc, this, Style, dx, dy);
            }

            if (mode.HasFlag(DrawMode.Point))
            {
                if (renderer.SelectionState.IsSelected(TopLeft))
                {
                    TopLeft.Draw(dc, renderer, dx, dy, mode, db, r);
                }

                if (renderer.SelectionState.IsSelected(BottomRight))
                {
                    BottomRight.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }

            base.Draw(dc, renderer, dx, dy, mode, db, r);
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new TextShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Style = this.Style,
                Text = (Text)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }
}

namespace Draw2D.ViewModels.Containers
{
    public interface IDrawContainerView : IDisposable
    {
        Dictionary<string, IShapeDecorator> Decorators { get; set; }
        void Draw(IContainerView view, object context, double width, double height, double dx, double dy, double zx, double zy);
    }

    public interface IHitTestable
    {
        IHitTest HitTest { get; set; }
        PointShape GetNextPoint(double x, double y, bool connect, double radius);
    }

    public interface ICanvasContainer : IDirty, IDrawable, ISelectable, ICopyable
    {
        IList<BaseShape> Shapes { get; set; }
        void GetPoints(IList<PointShape> points);
    }

    public interface IContainerView : IDrawTarget, IHitTestable
    {
        string Title { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        ArgbColor PrintBackground { get; set; }
        ArgbColor WorkBackground { get; set; }
        ArgbColor InputBackground { get; set; }
        IList<ShapeStyle> Styles { get; set; }
        ShapeStyle CurrentStyle { get; set; }
        BaseShape PointTemplate { get; set; }
        IDrawContainerView DrawContainerView { get; set; }
        ISelectionState SelectionState { get; set; }
        IZoomServiceState ZoomServiceState { get; set; }
        ICanvasContainer CurrentContainer { get; set; }
        ICanvasContainer WorkingContainer { get; set; }
    }

    public interface IToolContext : IInputTarget
    {
        IContainerView ContainerView { get; set; }
        IList<ITool> Tools { get; set; }
        ITool CurrentTool { get; set; }
        EditMode Mode { get; set; }
        void SetTool(string name);
    }

    [DataContract(IsReference = true)]
    public class CanvasContainer : BaseShape, ICanvasContainer, ICopyable
    {
        private IList<BaseShape> _shapes;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public CanvasContainer()
        {
        }

        public override void GetPoints(IList<PointShape> points)
        {
            foreach (var shape in Shapes)
            {
                shape.GetPoints(points);
            }
        }

        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, DrawMode mode, object db, object r)
        {
            if (Shapes != null)
            {
                foreach (var shape in Shapes)
                {
                    shape.Draw(dc, renderer, dx, dy, mode, db, r);
                }
            }
        }

        public override void Invalidate()
        {
            var points = new List<PointShape>();
            GetPoints(points);

            if (Shapes != null)
            {
                foreach (var shape in Shapes)
                {
                    shape.Invalidate();
                    shape.Style?.Invalidate();
                    shape.Style?.Stroke?.Invalidate();
                    shape.Style?.Fill?.Invalidate();
                    shape.Style?.TextStyle?.Invalidate();
                    shape.Style?.TextStyle?.Stroke?.Invalidate();
                }
            }

            foreach (var point in points)
            {
                point.Invalidate();
                point.Style?.Invalidate();
                point.Template?.Invalidate();
                point.Template?.Style?.Invalidate();
                point.Template?.Style?.Stroke?.Invalidate();
                point.Template?.Style?.Fill?.Invalidate();
                point.Template?.Style?.TextStyle?.Invalidate();
                point.Template?.Style?.TextStyle?.Stroke?.Invalidate();
            }

            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            var copy = new CanvasContainer()
            {
                Shapes = new ObservableCollection<BaseShape>(),
                Name = this.Name,
                Style = this.Style
            };

            foreach (var shape in this.Shapes)
            {
                copy.Shapes.Add((BaseShape)shape.Copy(shared));
            }

            return copy;
        }

        public override void Move(ISelectionState selectionState, double dx, double dy)
        {
            var points = new List<PointShape>();
            GetPoints(points);
            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                if (!selectionState.IsSelected(point))
                {
                    point.Move(selectionState, dx, dy);
                }
            }
        }
    }

    [DataContract(IsReference = true)]
    public class ZoomServiceState : ViewModelBase, IZoomServiceState, ICopyable
    {
        private double _zoomSpeed;
        private double _zoomX;
        private double _zoomY;
        private double _offsetX;
        private double _offsetY;
        private bool _isPanning;
        private bool _isZooming;
        private bool _customDraw;
        private FitMode _initFitMode;
        private FitMode _autoFitMode;

        public ZoomServiceState()
        {
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomSpeed
        {
            get => _zoomSpeed;
            set => Update(ref _zoomSpeed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomX
        {
            get => _zoomX;
            set => Update(ref _zoomX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomY
        {
            get => _zoomY;
            set => Update(ref _zoomY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsPanning
        {
            get => _isPanning;
            set => Update(ref _isPanning, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsZooming
        {
            get => _isZooming;
            set => Update(ref _isZooming, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool CustomDraw
        {
            get => _customDraw;
            set => Update(ref _customDraw, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FitMode InitFitMode
        {
            get => _initFitMode;
            set => Update(ref _initFitMode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FitMode AutoFitMode
        {
            get => _autoFitMode;
            set => Update(ref _autoFitMode, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            return new ZoomServiceState()
            {
                Name = this.Name,
                ZoomSpeed = this.ZoomSpeed,
                ZoomX = this.ZoomX,
                ZoomY = this.ZoomY,
                OffsetX = this.OffsetX,
                OffsetY = this.OffsetY,
                IsPanning = this.IsPanning,
                IsZooming = this.IsZooming,
                CustomDraw = this.CustomDraw,
                InitFitMode = this.InitFitMode,
                AutoFitMode = this.AutoFitMode
            };
        }
    }

    [DataContract(IsReference = true)]
    public class ContainerView : ViewModelBase, IContainerView, ICopyable
    {
        private string _title;
        private double _width;
        private double _height;
        private ArgbColor _printBackground;
        private ArgbColor _workBackground;
        private ArgbColor _inputBackground;
        private IList<ShapeStyle> _styles;
        private ShapeStyle _currentStyle;
        private BaseShape _pointTemplate;
        private IInputService _inputService;
        private IZoomService _zoomService;
        private IDrawContainerView _drawContainerView;
        private ISelectionState _selectionState;
        private IZoomServiceState _zoomServiceState;
        private ICanvasContainer _currentContainer;
        private ICanvasContainer _workingContainer;
        private IHitTest _hitTest;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Title
        {
            get => _title;
            set => Update(ref _title, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor PrintBackground
        {
            get => _printBackground;
            set => Update(ref _printBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor WorkBackground
        {
            get => _workBackground;
            set => Update(ref _workBackground, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor InputBackground
        {
            get => _inputBackground;
            set => Update(ref _inputBackground, value);
        }

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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BaseShape PointTemplate
        {
            get => _pointTemplate;
            set => Update(ref _pointTemplate, value);
        }

        [IgnoreDataMember]
        public IInputService InputService
        {
            get => _inputService;
            set => Update(ref _inputService, value);
        }

        [IgnoreDataMember]
        public IZoomService ZoomService
        {
            get => _zoomService;
            set => Update(ref _zoomService, value);
        }

        [IgnoreDataMember]
        public IDrawContainerView DrawContainerView
        {
            get => _drawContainerView;
            set => Update(ref _drawContainerView, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISelectionState SelectionState
        {
            get => _selectionState;
            set => Update(ref _selectionState, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IZoomServiceState ZoomServiceState
        {
            get => _zoomServiceState;
            set => Update(ref _zoomServiceState, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ICanvasContainer CurrentContainer
        {
            get => _currentContainer;
            set => Update(ref _currentContainer, value);
        }

        [IgnoreDataMember]
        public ICanvasContainer WorkingContainer
        {
            get => _workingContainer;
            set => Update(ref _workingContainer, value);
        }

        [IgnoreDataMember]
        public IHitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public virtual PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            if (connect == true)
            {
                var point = HitTest.TryToGetPoint(_currentContainer.Shapes, new Point2(x, y), radius, null);
                if (point != null)
                {
                    return point;
                }
            }
            return new PointShape(x, y, _pointTemplate);
        }

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _drawContainerView?.Draw(this, context, width, height, dx, dy, zx, zy);
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new ContainerView()
            {
                Name = this.Name,
                Title = this.Title,
                Width = this.Width,
                Height = this.Height,
                PrintBackground = (ArgbColor)this.PrintBackground?.Copy(shared),
                WorkBackground = (ArgbColor)this.WorkBackground?.Copy(shared),
                InputBackground = (ArgbColor)this.InputBackground?.Copy(shared),
                Styles = new ObservableCollection<ShapeStyle>(),
                CurrentStyle = (ShapeStyle)this.CurrentStyle?.Copy(shared),
                PointTemplate = (BaseShape)this.PointTemplate?.Copy(shared),
                DrawContainerView = null,
                SelectionState = (ISelectionState)this.SelectionState?.Copy(shared),
                ZoomServiceState = (IZoomServiceState)this.ZoomServiceState?.Copy(shared),
                CurrentContainer = (ICanvasContainer)this.CurrentContainer?.Copy(shared),
                WorkingContainer = null
            };

            foreach (var style in this.Styles)
            {
                if (style is ICopyable copyable)
                {
                    copy.Styles.Add((ShapeStyle)copyable.Copy(shared));
                }
            }

            return copy;
        }
    }

    [DataContract(IsReference = true)]
    public class ToolContext : ViewModelBase, IToolContext
    {
        private IList<IContainerView> _containerViews;
        private IContainerView _containerView;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private EditMode _mode;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<IContainerView> ContainerViews
        {
            get => _containerViews;
            set => Update(ref _containerViews, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IContainerView ContainerView
        {
            get => _containerView;
            set => Update(ref _containerView, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool?.Clean(this);
                Update(ref _currentTool, value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EditMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public void SetTool(string title)
        {
            if (CurrentTool is PathTool pathTool && pathTool.Settings.CurrentTool.Title != title)
            {
                pathTool.CleanCurrentTool(this);
                var tool = pathTool.Settings.Tools.Where(t => t.Title == title).FirstOrDefault();
                if (tool != null)
                {
                    pathTool.Settings.CurrentTool = tool;
                }
                else
                {
                    CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
                }
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == title).FirstOrDefault();
            }
        }

        public void LeftDown(double x, double y, Modifier modifier)
        {
            _currentTool.LeftDown(this, x, y, modifier);
        }

        public void LeftUp(double x, double y, Modifier modifier)
        {
            if (_mode == EditMode.Mouse)
            {
                _currentTool.LeftUp(this, x, y, modifier);
            }
            else if (_mode == EditMode.Touch)
            {
                _currentTool.LeftDown(this, x, y, modifier);
            }
        }

        public void RightDown(double x, double y, Modifier modifier)
        {
            _currentTool.RightDown(this, x, y, modifier);
        }

        public void RightUp(double x, double y, Modifier modifier)
        {
            _currentTool.RightUp(this, x, y, modifier);
        }

        public void Move(double x, double y, Modifier modifier)
        {
            _currentTool.Move(this, x, y, modifier);
        }

        public double GetWidth()
        {
            return ContainerView?.Width ?? 0.0;
        }

        public double GetHeight()
        {
            return ContainerView?.Height ?? 0.0;
        }
    }
}

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public abstract class CommonDecorator : IShapeDecorator
    {
        private readonly TextStyle _textStyle;
        private readonly ArgbColor _stroke;
        private readonly ArgbColor _fill;
        private readonly ShapeStyle _strokeStyle;
        private readonly ShapeStyle _fillStyle;
        private readonly LineShape _line;
        private readonly EllipseShape _ellipse;
        private readonly RectangleShape _rectangle;
        private readonly TextShape _text;

        public CommonDecorator()
        {
            _textStyle = new TextStyle("Calibri", 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true);
            _stroke = new ArgbColor(255, 0, 255, 255);
            _fill = new ArgbColor(255, 0, 255, 255);
            _strokeStyle = new ShapeStyle("Stroke", _stroke, _fill, 2.0, true, false, _textStyle);
            _fillStyle = new ShapeStyle("Fill", _stroke, _fill, 2.0, false, true, _textStyle);
            _line = new LineShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<PointShape>()
            };
            _ellipse = new EllipseShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<PointShape>(),
            };
            _rectangle = new RectangleShape(new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<PointShape>(),
            };
            _text = new TextShape(new Text(""), new PointShape(0, 0, null), new PointShape(0, 0, null))
            {
                Points = new ObservableCollection<PointShape>(),
            };
        }

        public void DrawLine(object dc, IShapeRenderer renderer, PointShape a, PointShape b, double dx, double dy, DrawMode mode)
        {
            _line.Style = _strokeStyle;
            _line.StartPoint.X = a.X;
            _line.StartPoint.Y = a.Y;
            _line.Point.X = b.X;
            _line.Point.Y = b.Y;
            _line.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void FillEllipse(object dc, IShapeRenderer renderer, PointShape s, double radius, double dx, double dy, DrawMode mode)
        {
            _ellipse.Style = _fillStyle;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void DrawEllipse(object dc, IShapeRenderer renderer, PointShape s, double radius, double dx, double dy, DrawMode mode)
        {
            _ellipse.Style = _strokeStyle;
            _ellipse.TopLeft.X = s.X - radius;
            _ellipse.TopLeft.Y = s.Y - radius;
            _ellipse.BottomRight.X = s.X + radius;
            _ellipse.BottomRight.Y = s.Y + radius;
            _ellipse.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void FillEllipse(object dc, IShapeRenderer renderer, PointShape a, PointShape b, double dx, double dy, DrawMode mode)
        {
            _ellipse.Style = _fillStyle;
            _ellipse.TopLeft.X = a.X;
            _ellipse.TopLeft.Y = a.Y;
            _ellipse.BottomRight.X = b.X;
            _ellipse.BottomRight.Y = b.Y;
            _ellipse.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void DrawEllipse(object dc, IShapeRenderer renderer, PointShape a, PointShape b, double dx, double dy, DrawMode mode)
        {
            _ellipse.Style = _strokeStyle;
            _ellipse.TopLeft.X = a.X;
            _ellipse.TopLeft.Y = a.Y;
            _ellipse.BottomRight.X = b.X;
            _ellipse.BottomRight.Y = b.Y;
            _ellipse.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void FillRectangle(object dc, IShapeRenderer renderer, PointShape s, double radius, double dx, double dy, DrawMode mode)
        {
            _rectangle.Style = _fillStyle;
            _rectangle.TopLeft.X = s.X - radius;
            _rectangle.TopLeft.Y = s.Y - radius;
            _rectangle.BottomRight.X = s.X + radius;
            _rectangle.BottomRight.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void DrawRectangle(object dc, IShapeRenderer renderer, PointShape s, double radius, double dx, double dy, DrawMode mode)
        {
            _rectangle.Style = _strokeStyle;
            _rectangle.TopLeft.X = s.X - radius;
            _rectangle.TopLeft.Y = s.Y - radius;
            _rectangle.BottomRight.X = s.X + radius;
            _rectangle.BottomRight.Y = s.Y + radius;
            _rectangle.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void FillRectangle(object dc, IShapeRenderer renderer, PointShape a, PointShape b, double dx, double dy, DrawMode mode)
        {
            _rectangle.Style = _fillStyle;
            _rectangle.TopLeft.X = a.X;
            _rectangle.TopLeft.Y = a.Y;
            _rectangle.BottomRight.X = b.X;
            _rectangle.BottomRight.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void DrawRectangle(object dc, IShapeRenderer renderer, PointShape a, PointShape b, double dx, double dy, DrawMode mode)
        {
            _rectangle.Style = _strokeStyle;
            _rectangle.TopLeft.X = a.X;
            _rectangle.TopLeft.Y = a.Y;
            _rectangle.BottomRight.X = b.X;
            _rectangle.BottomRight.Y = b.Y;
            _rectangle.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public void DrawText(object dc, IShapeRenderer renderer, string text, PointShape a, PointShape b, double dx, double dy, DrawMode mode)
        {
            _text.Style = _strokeStyle;
            _text.TopLeft.X = a.X;
            _text.TopLeft.Y = a.Y;
            _text.BottomRight.X = b.X;
            _text.BottomRight.Y = b.Y;
            _text.Draw(dc, renderer, dx, dy, mode, null, null);
        }

        public abstract void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode);
    }

    [DataContract(IsReference = true)]
    public class CubicBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, CubicBezierShape cubicBezier, double dx, double dy, DrawMode mode)
        {
            DrawLine(dc, renderer, cubicBezier.StartPoint, cubicBezier.Point1, dx, dy, mode);
            DrawLine(dc, renderer, cubicBezier.Point3, cubicBezier.Point2, dx, dy, mode);
            DrawLine(dc, renderer, cubicBezier.Point1, cubicBezier.Point2, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is CubicBezierShape cubicBezier)
            {
                Draw(dc, renderer, cubicBezier, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, EllipseShape ellipseShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, ellipseShape.TopLeft, ellipseShape.BottomRight, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is EllipseShape ellipseShape)
            {
                Draw(dc, renderer, ellipseShape, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class LineDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, LineShape lineShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, lineShape.StartPoint, lineShape.Point, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is LineShape lineShape)
            {
                Draw(dc, renderer, lineShape, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class PathDecorator : CommonDecorator
    {
        private readonly LineDecorator _lineDecorator;
        private readonly CubicBezierDecorator _cubiceBezierDecorator;
        private readonly QuadraticBezierDecorator _quadraticBezierDecorator;
        private readonly ConicDecorator _conicDecorator;

        public PathDecorator()
        {
            _lineDecorator = new LineDecorator();
            _cubiceBezierDecorator = new CubicBezierDecorator();
            _quadraticBezierDecorator = new QuadraticBezierDecorator();
            _conicDecorator = new ConicDecorator();
        }

        public void DrawShape(object dc, IShapeRenderer renderer, BaseShape shape, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is LineShape line)
            {
                if (selectionState.IsSelected(line))
                {
                    _lineDecorator.Draw(dc, line, renderer, selectionState, dx, dy, mode);
                }
            }
            else if (shape is CubicBezierShape cubicBezier)
            {
                if (selectionState.IsSelected(cubicBezier))
                {
                    _cubiceBezierDecorator.Draw(dc, cubicBezier, renderer, selectionState, dx, dy, mode);
                }
            }
            else if (shape is QuadraticBezierShape quadraticBezier)
            {
                if (selectionState.IsSelected(quadraticBezier))
                {
                    _quadraticBezierDecorator.Draw(dc, quadraticBezier, renderer, selectionState, dx, dy, mode);
                }
            }
            else if (shape is ConicShape conicShape)
            {
                if (selectionState.IsSelected(conicShape))
                {
                    _conicDecorator.Draw(dc, conicShape, renderer, selectionState, dx, dy, mode);
                }
            }
        }

        public void DrawFigure(object dc, IShapeRenderer renderer, FigureShape figure, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            foreach (var shape in figure.Shapes)
            {
                DrawShape(dc, renderer, shape, selectionState, dx, dy, mode);
            }
        }

        public void Draw(object dc, IShapeRenderer renderer, PathShape path, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            foreach (var figure in path.Figures)
            {
                DrawFigure(dc, renderer, figure, selectionState, dx, dy, mode);
            }
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is PathShape path)
            {
                Draw(dc, renderer, path, selectionState, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class PointDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, PointShape pointShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, pointShape, 10, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is PointShape pointShape)
            {
                Draw(dc, renderer, pointShape, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, QuadraticBezierShape quadraticBezier, double dx, double dy, DrawMode mode)
        {
            DrawLine(dc, renderer, quadraticBezier.StartPoint, quadraticBezier.Point1, dx, dy, mode);
            DrawLine(dc, renderer, quadraticBezier.Point1, quadraticBezier.Point2, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is QuadraticBezierShape quadraticBezier)
            {
                Draw(dc, renderer, quadraticBezier, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class ConicDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ConicShape conic, double dx, double dy, DrawMode mode)
        {
            DrawLine(dc, renderer, conic.StartPoint, conic.Point1, dx, dy, mode);
            DrawLine(dc, renderer, conic.Point1, conic.Point2, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is ConicShape conic)
            {
                Draw(dc, renderer, conic, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, RectangleShape rectangleShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, rectangleShape.TopLeft, rectangleShape.BottomRight, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is RectangleShape rectangleShape)
            {
                Draw(dc, renderer, rectangleShape, dx, dy, mode);
            }
        }
    }

    [DataContract(IsReference = true)]
    public class TextDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, TextShape textShape, double dx, double dy, DrawMode mode)
        {
            DrawRectangle(dc, renderer, textShape.TopLeft, textShape.BottomRight, dx, dy, mode);
        }

        public override void Draw(object dc, BaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, DrawMode mode)
        {
            if (shape is TextShape textShape)
            {
                Draw(dc, renderer, textShape, dx, dy, mode);
            }
        }
    }
}

namespace Draw2D.ViewModels.Filters
{
    [Flags]
    public enum GridSnapMode
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        All = Horizontal | Vertical
    }

    [DataContract(IsReference = true)]
    public class GridSnapSettings : Settings
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private GridSnapMode _mode;
        private double _gridSizeX;
        private double _gridSizeY;
        private ShapeStyle _guideStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GridSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double GridSizeX
        {
            get => _gridSizeX;
            set => Update(ref _gridSizeX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double GridSizeY
        {
            get => _gridSizeY;
            set => Update(ref _gridSizeY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShapeStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class GridSnapPointFilter : PointFilter
    {
        private GridSnapSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Grid-Snap";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public GridSnapSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.IsEnabled == false)
            {
                return false;
            }

            if (Settings.Mode != GridSnapMode.None)
            {
                bool haveSnapToGrid = false;

                if (Settings.Mode.HasFlag(GridSnapMode.Horizontal))
                {
                    x = SnapGrid(x, Settings.GridSizeX);
                    haveSnapToGrid = true;
                }

                if (Settings.Mode.HasFlag(GridSnapMode.Vertical))
                {
                    y = SnapGrid(y, Settings.GridSizeY);
                    haveSnapToGrid = true;
                }

                if (Settings.EnableGuides && haveSnapToGrid)
                {
                    PointGuides(context, x, y);
                }

                return haveSnapToGrid;
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.ContainerView?.Width ?? 0, y, null),
                Style = Settings.GuideStyle
            };

            var vertical = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.ContainerView?.Height ?? 0, null),
                Style = Settings.GuideStyle
            };

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.ContainerView?.WorkingContainer.Shapes.Add(horizontal);
            context.ContainerView?.WorkingContainer.Shapes.Add(vertical);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
        }

        public static double SnapGrid(double value, double size)
        {
            double r = value % size;
            return r >= size / 2.0 ? value + size - r : value - r;
        }
    }

    [Flags]
    public enum LineSnapMode
    {
        None = 0,
        Point = 1,
        Middle = 2,
        Intersection = 4,
        Horizontal = 8,
        Vertical = 16,
        Nearest = 32,
        All = Point | Middle | Intersection | Horizontal | Vertical | Nearest
    }

    [Flags]
    public enum LineSnapTarget
    {
        None = 0,
        Shapes = 1,
        All = Shapes
    }

    [DataContract(IsReference = true)]
    public class LineSnapSettings : Settings
    {
        private bool _isEnabled;
        private bool _enableGuides;
        private LineSnapMode _mode;
        private LineSnapTarget _target;
        private double _threshold;
        private ShapeStyle _guideStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool EnableGuides
        {
            get => _enableGuides;
            set => Update(ref _enableGuides, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapTarget Target
        {
            get => _target;
            set => Update(ref _target, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Threshold
        {
            get => _threshold;
            set => Update(ref _threshold, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShapeStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class LineSnapPointFilter : PointFilter
    {
        private LineSnapSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Line-Snap";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineSnapSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override bool Process(IToolContext context, ref double x, ref double y)
        {
            if (Settings.IsEnabled == false)
            {
                return false;
            }

            if (Settings.Target.HasFlag(LineSnapTarget.Shapes))
            {
                if (Process(context, ref x, ref y, context.ContainerView?.CurrentContainer.Shapes.OfType<LineShape>()))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Process(IToolContext context, ref double x, ref double y, IEnumerable<LineShape> lines)
        {
            if (lines.Any() && Settings.Mode != LineSnapMode.None)
            {
                var result = SnapLines(lines, Settings.Mode, Settings.Threshold, new Point2(x, y), out var snap, out _);
                if (result)
                {
                    x = snap.X;
                    y = snap.Y;

                    if (Settings.EnableGuides)
                    {
                        Clear(context);
                        PointGuides(context, x, y);
                    }

                    return true;
                }
                Clear(context);
            }
            Clear(context);
            return false;
        }

        private void PointGuides(IToolContext context, double x, double y)
        {
            var horizontal = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.ContainerView?.Width ?? 0, y, null),
                Style = Settings.GuideStyle
            };

            var vertical = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.ContainerView?.Height ?? 0, null),
                Style = Settings.GuideStyle
            };

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.ContainerView?.WorkingContainer.Shapes.Add(horizontal);
            context.ContainerView?.WorkingContainer.Shapes.Add(vertical);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
        }

        public static bool SnapLinesToPoint(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line in lines)
            {
                var distance0 = line.StartPoint.ToPoint2().DistanceTo(point);
                if (distance0 < threshold)
                {
                    snap = new Point2(line.StartPoint.X, line.StartPoint.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
                var distance1 = line.Point.ToPoint2().DistanceTo(point);
                if (distance1 < threshold)
                {
                    snap = new Point2(line.Point.X, line.Point.Y);
                    result = LineSnapMode.Point;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToMiddle(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line in lines)
            {
                var middle = Line2.Middle(line.StartPoint.ToPoint2(), line.Point.ToPoint2());
                var distance = middle.DistanceTo(point);
                if (distance < threshold)
                {
                    snap = middle;
                    result = LineSnapMode.Middle;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLineToIntersection(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line0 in lines)
            {
                foreach (var line1 in lines)
                {
                    if (line0 == line1)
                    {
                        continue;
                    }

                    if (Line2.LineIntersectWithLine(line0.StartPoint.ToPoint2(), line0.Point.ToPoint2(), line1.StartPoint.ToPoint2(), line1.Point.ToPoint2(), out var clip))
                    {
                        var distance = clip.DistanceTo(point);
                        if (distance < threshold)
                        {
                            snap = clip;
                            result = LineSnapMode.Intersection;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool SnapLineToHorizontal(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result, out double horizontal)
        {
            snap = default;
            result = default;
            horizontal = default;

            foreach (var line in lines)
            {
                if (point.Y >= line.StartPoint.Y - threshold && point.Y <= line.StartPoint.Y + threshold)
                {
                    snap = new Point2(point.X, line.StartPoint.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.StartPoint.Y;
                    return true;
                }
                if (point.Y >= line.Point.Y - threshold && point.Y <= line.Point.Y + threshold)
                {
                    snap = new Point2(point.X, line.StartPoint.Y);
                    result |= LineSnapMode.Horizontal;
                    horizontal = line.Point.Y;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToVertical(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result, out double vertical)
        {
            snap = default;
            result = default;
            vertical = default;

            foreach (var line in lines)
            {
                if (point.X >= line.StartPoint.X - threshold && point.X <= line.StartPoint.X + threshold)
                {
                    snap = new Point2(line.StartPoint.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.StartPoint.X;
                    return true;
                }
                if (point.X >= line.Point.X - threshold && point.X <= line.Point.X + threshold)
                {
                    snap = new Point2(line.Point.X, point.Y);
                    result |= LineSnapMode.Vertical;
                    vertical = line.Point.X;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLinesToNearest(IEnumerable<LineShape> lines, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            foreach (var line in lines)
            {
                var nearest = point.NearestOnLine(line.StartPoint.ToPoint2(), line.Point.ToPoint2());
                var distance = nearest.DistanceTo(point);
                if (distance < threshold)
                {
                    snap = nearest;
                    result = LineSnapMode.Nearest;
                    return true;
                }
            }
            return false;
        }

        public static bool SnapLines(IEnumerable<LineShape> lines, LineSnapMode mode, double threshold, Point2 point, out Point2 snap, out LineSnapMode result)
        {
            snap = default;
            result = default;

            if (mode.HasFlag(LineSnapMode.Point))
            {
                if (SnapLinesToPoint(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Middle))
            {
                if (SnapLineToMiddle(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Intersection))
            {
                if (SnapLineToIntersection(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            double horizontal = default;
            double vertical = default;

            if (mode.HasFlag(LineSnapMode.Horizontal))
            {
                SnapLineToHorizontal(lines, threshold, point, out snap, out result, out horizontal);
            }

            if (mode.HasFlag(LineSnapMode.Vertical))
            {
                SnapLinesToVertical(lines, threshold, point, out snap, out result, out vertical);
            }

            if (result.HasFlag(LineSnapMode.Horizontal) || result.HasFlag(LineSnapMode.Vertical))
            {
                if (result.HasFlag(LineSnapMode.Vertical) || result.HasFlag(LineSnapMode.Horizontal))
                {
                    double x = result.HasFlag(LineSnapMode.Vertical) ? vertical : point.X;
                    double y = result.HasFlag(LineSnapMode.Horizontal) ? horizontal : point.Y;
                    snap = new Point2(x, y);
                    return true;
                }
            }

            if (mode.HasFlag(LineSnapMode.Nearest))
            {
                if (SnapLinesToNearest(lines, threshold, point, out snap, out result))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

namespace Draw2D.ViewModels.Intersections
{
    [DataContract(IsReference = true)]
    public class EllipseLineSettings : Settings
    {
        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseLineIntersection : PointIntersection
    {
        private EllipseLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Ellipse-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EllipseLineSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Find(IToolContext context, BaseShape shape)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            if (!Settings.IsEnabled)
            {
                return;
            }

            var ellipses = context.ContainerView?.CurrentContainer.Shapes.OfType<EllipseShape>();
            if (ellipses.Any())
            {
                foreach (var ellipse in ellipses)
                {
                    var rect = Rect2.FromPoints(ellipse.TopLeft.ToPoint2(), ellipse.BottomRight.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    Line2.LineIntersectsWithEllipse(p1, p2, rect, true, out var intersections);
                    if (intersections != null && intersections.Any())
                    {
                        foreach (var p in intersections)
                        {
                            var point = new PointShape(p.X, p.Y, context.ContainerView?.PointTemplate);
                            Intersections.Add(point);
                            context.ContainerView?.WorkingContainer.Shapes.Add(point);
                            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                            context.ContainerView?.SelectionState?.Select(point);
                        }
                    }
                }
            }
        }
    }

    [DataContract(IsReference = true)]
    public class LineLineSettings : Settings
    {
        private bool _isEnabled;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class LineLineIntersection : PointIntersection
    {
        private LineLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Line-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineLineSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Find(IToolContext context, BaseShape shape)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            if (!Settings.IsEnabled)
            {
                return;
            }

            var lines = context.ContainerView?.CurrentContainer.Shapes.OfType<LineShape>();
            if (lines.Any())
            {
                var a0 = line.StartPoint.ToPoint2();
                var b0 = line.Point.ToPoint2();
                foreach (var l in lines)
                {
                    var a1 = l.StartPoint.ToPoint2();
                    var b1 = l.Point.ToPoint2();
                    bool intersection = Line2.LineIntersectWithLine(a0, b0, a1, b1, out var clip);
                    if (intersection)
                    {
                        var point = new PointShape(clip.X, clip.Y, context.ContainerView?.PointTemplate);
                        Intersections.Add(point);
                        context.ContainerView?.WorkingContainer.Shapes.Add(point);
                        context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point);
                    }
                }
            }
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleLineSettings : Settings
    {
        private bool _isEnabled;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleLineIntersection : PointIntersection
    {
        private RectangleLineSettings _settings;

        [IgnoreDataMember]
        public override string Title => "Rectangle-Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public RectangleLineSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public override void Find(IToolContext context, BaseShape shape)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            if (!Settings.IsEnabled)
            {
                return;
            }

            var rectangles = context.ContainerView?.CurrentContainer.Shapes.OfType<RectangleShape>();
            if (rectangles.Any())
            {
                foreach (var rectangle in rectangles)
                {
                    var rect = Rect2.FromPoints(rectangle.TopLeft.ToPoint2(), rectangle.BottomRight.ToPoint2());
                    var p1 = line.StartPoint.ToPoint2();
                    var p2 = line.Point.ToPoint2();
                    var intersections = Line2.LineIntersectsWithRect(p1, p2, rect, out double x0clip, out double y0clip, out double x1clip, out double y1clip);
                    if (intersections)
                    {
                        var point1 = new PointShape(x0clip, y0clip, context.ContainerView?.PointTemplate);
                        Intersections.Add(point1);
                        context.ContainerView?.WorkingContainer.Shapes.Add(point1);
                        context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point1);

                        var point2 = new PointShape(x1clip, y1clip, context.ContainerView?.PointTemplate);
                        Intersections.Add(point2);
                        context.ContainerView?.WorkingContainer.Shapes.Add(point2);
                        context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                        context.ContainerView?.SelectionState?.Select(point2);
                    }
                }
            }
        }
    }
}

namespace Draw2D.ViewModels.Bounds
{
    [DataContract(IsReference = true)]
    public class CubicBezierBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(CubicBezierShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(cubicBezier.StartPoint, target, radius, hitTest) != null)
            {
                return cubicBezier.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point1, target, radius, hitTest) != null)
            {
                return cubicBezier.Point1;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point2, target, radius, hitTest) != null)
            {
                return cubicBezier.Point2;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point3, target, radius, hitTest) != null)
            {
                return cubicBezier.Point3;
            }

            foreach (var point in cubicBezier.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<PointShape>();
            cubicBezier.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is CubicBezierShape cubicBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<PointShape>();
            cubicBezier.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(EllipseShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");
            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class GroupBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(GroupShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            foreach (var groupPoint in group.Points)
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, hitTest) != null)
                {
                    return groupPoint;
                }
            }

            foreach (var groupShape in group.Shapes)
            {
                var groupHitTest = hitTest.Registered[groupShape.GetType().Name];
                var result = groupHitTest.TryToGetPoint(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupShape in group.Shapes)
            {
                var groupHitTest = hitTest.Registered[groupShape.GetType().Name];
                var result = groupHitTest.Contains(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is GroupShape group))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var groupShape in group.Shapes)
            {
                var groupHitTest = hitTest.Registered[groupShape.GetType().Name];
                var result = groupHitTest.Overlaps(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }
    }

    [DataContract(IsReference = true)]
    public class LineBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(LineShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(line.StartPoint, target, radius, hitTest) != null)
            {
                return line.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(line.Point, target, radius, hitTest) != null)
            {
                return line.Point;
            }

            foreach (var point in line.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            var nearest = target.NearestOnLine(a, b);
            double distance = target.DistanceTo(nearest);
            return distance < radius ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is LineShape line))
            {
                throw new ArgumentNullException("shape");
            }

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            return Line2.LineIntersectsWithRect(a, b, target, out _, out _, out _, out _) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class PathBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(PathShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException("shape");
            }

            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            foreach (var pathPoint in path.Points)
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, hitTest) != null)
                {
                    return pathPoint;
                }
            }

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var figureHitTest = hitTest.Registered[figureShape.GetType().Name];
                    var result = figureHitTest.TryToGetPoint(figureShape, target, radius, hitTest);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var figureHitTest = hitTest.Registered[figureShape.GetType().Name];
                    var result = figureHitTest.Contains(figureShape, target, radius, hitTest);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            var points = new List<PointShape>();
            path.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException("shape");
            }

            foreach (var figure in path.Figures)
            {
                foreach (var figureShape in figure.Shapes)
                {
                    var figureHitTest = hitTest.Registered[figureShape.GetType().Name];
                    var result = figureHitTest.Overlaps(figureShape, target, radius, hitTest);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            var points = new List<PointShape>();
            path.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class PointBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(PointShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is PointShape point))
            {
                throw new ArgumentNullException("shape");
            }

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(QuadraticBezierShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(quadraticBezier.StartPoint, target, radius, hitTest) != null)
            {
                return quadraticBezier.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(quadraticBezier.Point1, target, radius, hitTest) != null)
            {
                return quadraticBezier.Point1;
            }

            if (pointHitTest.TryToGetPoint(quadraticBezier.Point2, target, radius, hitTest) != null)
            {
                return quadraticBezier.Point2;
            }

            foreach (var point in quadraticBezier.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<PointShape>();
            quadraticBezier.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is QuadraticBezierShape quadraticBezier))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<PointShape>();
            quadraticBezier.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class ConicBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(ConicShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(conic.StartPoint, target, radius, hitTest) != null)
            {
                return conic.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(conic.Point1, target, radius, hitTest) != null)
            {
                return conic.Point1;
            }

            if (pointHitTest.TryToGetPoint(conic.Point2, target, radius, hitTest) != null)
            {
                return conic.Point2;
            }

            foreach (var point in conic.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<PointShape>();
            conic.GetPoints(points);

            return HitTestHelper.Contains(points, target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            if (!(shape is ConicShape conic))
            {
                throw new ArgumentNullException("shape");
            }

            var points = new List<PointShape>();
            conic.GetPoints(points);

            return HitTestHelper.Overlap(points, target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(RectangleShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");
            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }

    [DataContract(IsReference = true)]
    public class TextBounds : IBounds
    {
        [IgnoreDataMember]
        public string TargetType => typeof(TextShape).Name;

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");
            var pointHitTest = hitTest.Registered[typeof(PointShape).Name];

            if (pointHitTest.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }

    public static class HitTestHelper
    {
        public static MonotoneChain MC => new MonotoneChain();

        public static SeparatingAxisTheorem SAT => new SeparatingAxisTheorem();

        public static Vector2[] ToSelection(Rect2 rect)
        {
            return new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };
        }

        public static void ToConvexHull(IEnumerable<PointShape> points, out int k, out Vector2[] convexHull)
        {
            Vector2[] vertices = new Vector2[points.Count()];
            int i = 0;
            foreach (var point in points)
            {
                vertices[i] = new Vector2(point.X, point.Y);
                i++;
            }
            MC.ConvexHull(vertices, out convexHull, out k);
        }

        public static bool Contains(IEnumerable<PointShape> points, Point2 point)
        {
            ToConvexHull(points, out int k, out Vector2[] convexHull);
            bool contains = false;
            for (int i = 0, j = k - 2; i < k - 1; j = i++)
            {
                if (((convexHull[i].Y > point.Y) != (convexHull[j].Y > point.Y))
                    && (point.X < (convexHull[j].X - convexHull[i].X) * (point.Y - convexHull[i].Y) / (convexHull[j].Y - convexHull[i].Y) + convexHull[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        public static bool Overlap(IEnumerable<PointShape> points, Vector2[] selectionState)
        {
            ToConvexHull(points, out int k, out Vector2[] convexHull);
            Vector2[] vertices = convexHull.Take(k).ToArray();
            return SAT.Overlap(selectionState, vertices);
        }

        public static bool Overlap(IEnumerable<PointShape> points, Rect2 rect)
        {
            return Overlap(points, ToSelection(rect));
        }
    }

    [DataContract(IsReference = true)]
    public class HitTest : ViewModelBase, IHitTest
    {
        private Dictionary<string, IBounds> _registered;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Dictionary<string, IBounds> Registered
        {
            get => _registered;
            set => Update(ref _registered, value);
        }

        public HitTest()
        {
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public void Register(IBounds hitTest)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }

        private IBounds GetHitTest(object target)
        {
            return Registered.TryGetValue(target?.GetType().Name, out var hitTest) ? hitTest : null;
        }

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius)
        {
            return GetHitTest(shape)?.TryToGetPoint(shape, target, radius, this);
        }

        public PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, PointShape exclude)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = TryToGetPoint(shape, target, radius);
                if (result != null && result != exclude)
                {
                    return result;
                }
            }
            return null;
        }

        public BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = GetHitTest(shape)?.Contains(shape, target, radius, this);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<BaseShape>();
            foreach (var shape in shapes.Reverse())
            {
                var result = GetHitTest(shape)?.Overlaps(shape, target, radius, this);
                if (result != null)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class CubicBezierToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class CubicBezierTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private CubicBezierToolSettings _settings;
        private CubicBezierShape _cubicBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2,
            Point3
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "CubicBezier";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public CubicBezierToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            _cubicBezier = new CubicBezierShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = next,
                Point1 = (PointShape)next.Copy(null),
                Point2 = (PointShape)next.Copy(null),
                Point3 = (PointShape)next.Copy(null),
                Text = new Text(),
                Style = context.ContainerView?.CurrentStyle
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_cubicBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_cubicBezier);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.StartPoint);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point1);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point2);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point3);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point3;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_cubicBezier);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point1);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_cubicBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            _cubicBezier.Point1 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.CurrentContainer.Shapes.Add(_cubicBezier);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _cubicBezier = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
            _cubicBezier.Point2 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
            _cubicBezier.Point3 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.SelectionState?.Select(_cubicBezier.Point3);

            CurrentState = State.Point2;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point1.X = x;
            _cubicBezier.Point1.Y = y;
            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint3Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _cubicBezier.Point2.X = x;
            _cubicBezier.Point2.Y = y;
            _cubicBezier.Point3.X = x;
            _cubicBezier.Point3.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_cubicBezier != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_cubicBezier);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point1);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point2);
                context.ContainerView?.SelectionState?.Deselect(_cubicBezier.Point3);
                _cubicBezier = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point1:
                    {
                        Point1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        Point2Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point3:
                    {
                        Point3Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point1:
                case State.Point2:
                case State.Point3:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point1:
                    {
                        MovePoint1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        MovePoint2Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point3:
                    {
                        MovePoint3Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class EllipseTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private EllipseToolSettings _settings;
        private EllipseShape _ellipse = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Ellipse";


        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EllipseToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _ellipse = new EllipseShape()
            {
                Points = new ObservableCollection<PointShape>(),
                TopLeft = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                BottomRight = context.ContainerView?.GetNextPoint(x, y, false, 0.0),
                Style = context.ContainerView?.CurrentStyle
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_ellipse);
            context.ContainerView?.SelectionState?.Select(_ellipse.TopLeft);
            context.ContainerView?.SelectionState?.Select(_ellipse.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_ellipse.BottomRight);
            _ellipse.BottomRight = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_ellipse);
            context.ContainerView?.SelectionState?.Deselect(_ellipse.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_ellipse);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _ellipse = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _ellipse.BottomRight.X = x;
            _ellipse.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            Filters?.ForEach(f => f.Clear(context));

            if (_ellipse != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_ellipse);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_ellipse);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_ellipse.BottomRight);
                _ellipse = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        BottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.BottomRight:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        MoveBottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class LineToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private bool _splitIntersections;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool SplitIntersections
        {
            get => _splitIntersections;
            set => Update(ref _splitIntersections, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    public static class LineHelper
    {
        public static IList<LineShape> SplitByIntersections(IToolContext context, IEnumerable<PointIntersection> intersections, LineShape target)
        {
            var points = intersections.SelectMany(i => i.Intersections).ToList();
            points.Insert(0, target.StartPoint);
            points.Insert(points.Count, target.Point);

            var unique = points
                .Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                .Select(p => new PointShape(p.X, p.Y, context.ContainerView?.PointTemplate)).ToList();

            var lines = new ObservableCollection<LineShape>();
            for (int i = 0; i < unique.Count - 1; i++)
            {
                var line = new LineShape(unique[i], unique[i + 1])
                {
                    Points = new ObservableCollection<PointShape>(),
                    Style = context.ContainerView?.CurrentStyle
                };
                context.ContainerView?.CurrentContainer.Shapes.Add(line);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                lines.Add(line);
            }

            return lines;
        }
    }

    [DataContract(IsReference = true)]
    public class LineTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private LineToolSettings _settings;
        private LineShape _line = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "Line";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LineToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);
            _line = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = next,
                Point = (PointShape)next.Copy(null),
                Text = new Text(),
                Style = context.ContainerView?.CurrentStyle
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_line);
            context.ContainerView?.SelectionState?.Select(_line.StartPoint);
            context.ContainerView?.SelectionState?.Select(_line.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_line);
            context.ContainerView?.SelectionState?.Deselect(_line.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_line.Point);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            _line.Point = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 0.0);

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            if ((Settings?.SplitIntersections ?? false) && (Intersections?.Any(i => i.Intersections.Count > 0) ?? false))
            {
                LineHelper.SplitByIntersections(context, Intersections, _line);
            }
            else
            {
                context.ContainerView?.CurrentContainer.Shapes.Add(_line);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _line = null;

            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _line.Point.X = x;
            _line.Point.Y = y;

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_line);
                context.ContainerView?.SelectionState?.Deselect(_line.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_line.Point);
                _line = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class MoveToolSettings : Settings
    {
        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class MoveTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private MoveToolSettings _settings;
        private PathTool _pathTool;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathTool PathTool
        {
            get => _pathTool;
            set => Update(ref _pathTool, value);
        }

        [IgnoreDataMember]
        public string Title => "Move";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public MoveToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public MoveTool(PathTool pathTool)
        {
            PathTool = pathTool;
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            PathTool.Move(context);
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Clean(IToolContext context)
        {
        }
    }

    [DataContract(IsReference = true)]
    public class NoneToolSettings : Settings
    {
        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class NoneTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private NoneToolSettings _settings;

        [IgnoreDataMember]
        public string Title => "None";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public NoneToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Clean(IToolContext context)
        {
        }
    }

    [DataContract(IsReference = true)]
    public class PathToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;
        private IList<ITool> _tools;
        private ITool _currentTool;
        private ITool _previousTool;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<ITool> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool CurrentTool
        {
            get => _currentTool;
            set
            {
                PreviousTool = _currentTool;
                Update(ref _currentTool, value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ITool PreviousTool
        {
            get => _previousTool;
            set => Update(ref _previousTool, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    internal class FigureContainerView : IContainerView
    {
        internal IToolContext _context;
        internal PathTool _pathTool;
        internal PointShape _nextPoint;

        public FigureContainerView(IToolContext context, PathTool pathTool)
        {
            _context = context;
            _pathTool = pathTool;
        }

        [IgnoreDataMember]
        public string Title
        {
            get => _context.ContainerView.Title;
            set => throw new InvalidOperationException($"Can not set {Title} property value.");
        }

        [IgnoreDataMember]
        public double Width
        {
            get => _context.ContainerView.Width;
            set => throw new InvalidOperationException($"Can not set {Width} property value.");
        }

        [IgnoreDataMember]
        public double Height
        {
            get => _context.ContainerView.Width;
            set => throw new InvalidOperationException($"Can not set {Height} property value.");
        }

        [IgnoreDataMember]
        public ArgbColor PrintBackground
        {
            get => _context.ContainerView.PrintBackground;
            set => throw new InvalidOperationException($"Can not set {PrintBackground} property value.");
        }

        [IgnoreDataMember]
        public ArgbColor WorkBackground
        {
            get => _context.ContainerView.WorkBackground;
            set => throw new InvalidOperationException($"Can not set {WorkBackground} property value.");
        }

        [IgnoreDataMember]
        public ArgbColor InputBackground
        {
            get => _context.ContainerView.InputBackground;
            set => throw new InvalidOperationException($"Can not set {InputBackground} property value.");
        }

        [IgnoreDataMember]
        public IList<ShapeStyle> Styles
        {
            get => _context.ContainerView.Styles;
            set => throw new InvalidOperationException($"Can not set {Styles} property value.");
        }

        [IgnoreDataMember]
        public ShapeStyle CurrentStyle
        {
            get => _context.ContainerView.CurrentStyle;
            set => throw new InvalidOperationException($"Can not set {CurrentStyle} property value.");
        }

        [IgnoreDataMember]
        public BaseShape PointTemplate
        {
            get => _context.ContainerView.PointTemplate;
            set => throw new InvalidOperationException($"Can not set {PointTemplate} property value.");
        }

        [IgnoreDataMember]
        public ICanvasContainer CurrentContainer
        {
            get => _pathTool._figure;
            set => throw new InvalidOperationException($"Can not set {CurrentContainer} property value.");
        }

        [IgnoreDataMember]
        public ICanvasContainer WorkingContainer
        {
            get => _pathTool._figure;
            set => throw new InvalidOperationException($"Can not set {WorkingContainer} property value.");
        }

        [IgnoreDataMember]
        public IDrawContainerView DrawContainerView
        {
            get => _context.ContainerView.DrawContainerView;
            set => throw new InvalidOperationException($"Can not set {DrawContainerView} property value.");
        }

        [IgnoreDataMember]
        public ISelectionState SelectionState
        {
            get => _context.ContainerView.SelectionState;
            set => throw new InvalidOperationException($"Can not set {SelectionState} property value.");
        }

        [IgnoreDataMember]
        public IZoomServiceState ZoomServiceState
        {
            get => _context.ContainerView.ZoomServiceState;
            set => throw new InvalidOperationException($"Can not set {ZoomServiceState} property value.");
        }

        [IgnoreDataMember]
        public IInputService InputService
        {
            get => _context.ContainerView?.InputService;
            set => throw new InvalidOperationException($"Can not set {InputService} property value.");
        }

        [IgnoreDataMember]
        public IZoomService ZoomService
        {
            get => _context.ContainerView.ZoomService;
            set => throw new InvalidOperationException($"Can not set {ZoomService} property value.");
        }

        [IgnoreDataMember]
        public IHitTest HitTest
        {
            get => _context.ContainerView.HitTest;
            set => throw new InvalidOperationException($"Can not set {HitTest} property value.");
        }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
            => _nextPoint ?? _context.ContainerView.GetNextPoint(x, y, connect, radius);

        public void Draw(object context, double width, double height, double dx, double dy, double zx, double zy)
        {
            _context.ContainerView.Draw(context, width, height, dx, dy, zx, zy);
        }
    }

    [DataContract(IsReference = true)]
    public partial class PathTool : IToolContext
    {
        internal IToolContext _context;
        internal FigureContainerView _containerView;

        [IgnoreDataMember]
        public IContainerView ContainerView
        {
            get => _containerView;
            set => throw new InvalidOperationException($"Can not set {ContainerView} property value.");
        }

        [IgnoreDataMember]
        public IList<ITool> Tools
        {
            get => _context.Tools;
            set => throw new InvalidOperationException($"Can not set {Tools} property value.");
        }

        [IgnoreDataMember]
        public ITool CurrentTool
        {
            get => _context.CurrentTool;
            set => throw new InvalidOperationException($"Can not set {CurrentTool} property value.");
        }

        [IgnoreDataMember]
        public EditMode Mode
        {
            get => _context.Mode;
            set => throw new InvalidOperationException($"Can not set {Mode} property value.");
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public void SetTool(string name)
            => _context.SetTool(name);

        public double GetWidth()
            => _context.GetWidth();

        public double GetHeight()
            => _context.GetHeight();

        public void LeftDown(double x, double y, Modifier modifier)
            => _context.LeftDown(x, y, modifier);

        public void LeftUp(double x, double y, Modifier modifier)
            => _context.LeftUp(x, y, modifier);

        public void RightDown(double x, double y, Modifier modifier)
            => _context.RightDown(x, y, modifier);

        public void RightUp(double x, double y, Modifier modifier)
            => _context.RightUp(x, y, modifier);

        public void Move(double x, double y, Modifier modifier)
            => _context.Move(x, y, modifier);

        private void SetContext(IToolContext context)
            => _context = context;

        private void SetNextPoint(PointShape point)
            => _containerView._nextPoint = point;
    }

    public partial class PathTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private PathToolSettings _settings;

        internal PathShape _path;
        internal FigureShape _figure;

        [IgnoreDataMember]
        public string Title => "Path";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public PointShape GetLastPoint()
        {
            if (_path?.Figures.Count > 0)
            {
                var shapes = _path.Figures[_path.Figures.Count - 1].Shapes;
                if (shapes.Count > 0)
                {
                    switch (shapes[shapes.Count - 1])
                    {
                        case LineShape line:
                            return line.Point;
                        case CubicBezierShape cubicBezier:
                            return cubicBezier.Point3;
                        case QuadraticBezierShape quadraticBezier:
                            return quadraticBezier.Point2;
                        case ConicShape conic:
                            return conic.Point2;
                        default:
                            throw new Exception("Could not find last path point.");
                    }
                }
            }
            return null;
        }

        public void Create(IToolContext context)
        {
            if (_containerView == null)
            {
                _containerView = new FigureContainerView(context, this);
            }

            _path = new PathShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Figures = new ObservableCollection<FigureShape>(),
                FillRule = Settings.FillRule,
                Text = new Text(),
                Style = context.ContainerView?.CurrentStyle
            };

            context.ContainerView?.WorkingContainer.Shapes.Add(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_path);
        }

        public void Move(IToolContext context)
        {
            _figure = new FigureShape()
            {
                Shapes = new ObservableCollection<BaseShape>(),
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };
            _path.Figures.Add(_figure);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            if (Settings.PreviousTool != null)
            {
                Settings.CurrentTool = Settings.PreviousTool;
            }
        }

        public void CleanCurrentTool(IToolContext context)
        {
            SetContext(context);
            Settings.CurrentTool?.Clean(this);
            SetContext(null);
        }

        public void UpdateCache(IToolContext context)
        {
            if (_path != null)
            {
                _figure.MarkAsDirty(true);
                _figure.MarkAsDirty(true);
            }
        }

        private void DownInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            if (_path == null)
            {
                Create(context);
                Move(context);
            }

            SetContext(context);
            Settings.CurrentTool?.LeftDown(this, x, y, modifier);

            switch (Settings.CurrentTool)
            {
                case LineTool lineTool:
                    {
                        if (lineTool.CurrentState == LineTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case CubicBezierTool cubicBezierTool:
                    {
                        if (cubicBezierTool.CurrentState == CubicBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
                case QuadraticBezierTool quadraticBezierTool:
                    {
                        if (quadraticBezierTool.CurrentState == QuadraticBezierTool.State.StartPoint)
                        {
                            SetNextPoint(GetLastPoint());
                            Settings.CurrentTool?.LeftDown(this, x, y, modifier);
                            SetNextPoint(null);
                        }
                    }
                    break;
            }

            SetContext(null);
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            if (_containerView == null)
            {
                _containerView = new FigureContainerView(context, this);
            }

            SetContext(context);
            Settings.CurrentTool.Move(this, x, y, modifier);
            SetContext(null);
        }

        private void CleanInternal(IToolContext context)
        {
            CleanCurrentTool(context);

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_path);

                if (_path.Validate(true) == true)
                {
                    context.ContainerView?.CurrentContainer.Shapes.Add(_path);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);
                }

                Settings.PreviousTool = null;
                SetNextPoint(null);
                SetContext(null);

                _path = null;
                _figure = null;
                _containerView = null;
            }
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            DownInternal(context, x, y, modifier);
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            this.Clean(context);
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            MoveInternal(context, x, y, modifier);
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class PointToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class PointTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private PointToolSettings _settings;

        [IgnoreDataMember]
        public string Title => "Point";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var point = new PointShape(x, y, context.ContainerView?.PointTemplate);

            var shape = context.ContainerView?.HitTest?.TryToGetShape(
                context.ContainerView?.CurrentContainer.Shapes,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0);
            if (shape != null && (Settings?.ConnectPoints ?? false))
            {
                if (shape is ConnectableShape connectable)
                {
                    connectable.Points.Add(point);
                    context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                    context.ContainerView?.SelectionState?.Select(point);
                    context.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
            //else
            //{
            //    context.ContainerView?.CurrentContainer.Shapes.Add(point);
            //    context.ContainerView?.InputService?.Redraw?.Invoke();
            //}
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            PointInternal(context, x, y, modifier);
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            MoveInternal(context, x, y, modifier);
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class PolyLineToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class PolyLineTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private PolyLineToolSettings _settings;
        private LineShape _line = null;
        private IList<PointShape> _points = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "PolyLine";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PolyLineToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _points = new ObservableCollection<PointShape>();
            _line = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                Point = context.ContainerView?.GetNextPoint(x, y, false, 0.0),
                Style = context?.ContainerView.CurrentStyle
            };
            _points.Add(_line.StartPoint);
            _points.Add(_line.Point);
            context.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_line);
            context.ContainerView?.SelectionState?.Select(_line.StartPoint);
            context.ContainerView?.SelectionState?.Select(_line.Point);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.SelectionState?.Deselect(_line);
            context.ContainerView?.SelectionState?.Deselect(_line.Point);
            _line.Point = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _points[_points.Count - 1] = _line.Point;

            if (!context.ContainerView?.SelectionState?.IsSelected(_line.Point) ?? false)
            {
                context.ContainerView?.SelectionState?.Select(_line.Point);
            }

            context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.CurrentContainer.Shapes.Add(_line);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);

            _line = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = _points.Last(),
                Point = context.ContainerView?.GetNextPoint(x, y, false, 0.0),
                Style = context.ContainerView?.CurrentStyle
            };
            _points.Add(_line.Point);
            context.ContainerView?.WorkingContainer.Shapes.Add(_line);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_line);
            context.ContainerView?.SelectionState?.Select(_line.Point);

            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _line.Point.X = x;
            _line.Point.Y = y;

            Intersections?.ForEach(i => i.Clear(context));
            Intersections?.ForEach(i => i.Find(context, _line));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Intersections?.ForEach(i => i.Clear(context));
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_line != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_line);
                context.ContainerView?.SelectionState?.Deselect(_line);
                _line = null;
            }

            if (_points != null)
            {
                _points.ForEach(point =>
                {
                    context.ContainerView?.SelectionState?.Deselect(point);
                });
                _points = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        PointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
                    {
                        MovePointInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class QuadraticBezierTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private QuadraticBezierToolSettings _settings;
        private QuadraticBezierShape _quadraticBezier = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "QuadraticBezier";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public QuadraticBezierToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            _quadraticBezier = new QuadraticBezierShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = next,
                Point1 = (PointShape)next.Copy(null),
                Point2 = (PointShape)next.Copy(null),
                Text = new Text(),
                Style = context.ContainerView?.CurrentStyle
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_quadraticBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.StartPoint);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point1);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point2);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point1);
            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_quadraticBezier);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            _quadraticBezier.Point1 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.CurrentContainer.Shapes.Add(_quadraticBezier);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _quadraticBezier = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
            _quadraticBezier.Point2 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.SelectionState?.Select(_quadraticBezier.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _quadraticBezier.Point1.X = x;
            _quadraticBezier.Point1.Y = y;
            _quadraticBezier.Point2.X = x;
            _quadraticBezier.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_quadraticBezier != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_quadraticBezier);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point1);
                context.ContainerView?.SelectionState?.Deselect(_quadraticBezier.Point2);
                _quadraticBezier = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point1:
                    {
                        Point1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        Point2Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point1:
                case State.Point2:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point1:
                    {
                        MovePoint1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        MovePoint2Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class ConicToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;
        private double _weight;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Weight
        {
            get => _weight;
            set => Update(ref _weight, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class ConicTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private ConicToolSettings _settings;
        private ConicShape _conic = null;

        public enum State
        {
            StartPoint,
            Point1,
            Point2
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public string Title => "Conic";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ConicToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void StartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var next = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            _conic = new ConicShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = next,
                Point1 = (PointShape)next.Copy(null),
                Point2 = (PointShape)next.Copy(null),
                Weight = Settings.Weight,
                Text = new Text(),
                Style = context.ContainerView?.CurrentStyle
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_conic);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_conic);
            context.ContainerView?.SelectionState?.Select(_conic.StartPoint);
            context.ContainerView?.SelectionState?.Select(_conic.Point1);
            context.ContainerView?.SelectionState?.Select(_conic.Point2);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point2;
        }

        private void Point1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.StartPoint;

            context.ContainerView?.SelectionState?.Deselect(_conic);
            context.ContainerView?.SelectionState?.Deselect(_conic.StartPoint);
            context.ContainerView?.SelectionState?.Deselect(_conic.Point1);
            context.ContainerView?.SelectionState?.Deselect(_conic.Point2);
            context.ContainerView?.WorkingContainer.Shapes.Remove(_conic);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            _conic.Point1 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.CurrentContainer.Shapes.Add(_conic);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _conic = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void Point2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _conic.Point1.X = x;
            _conic.Point1.Y = y;

            context.ContainerView?.SelectionState?.Deselect(_conic.Point2);
            _conic.Point2 = context.ContainerView?.GetNextPoint(x, y, false, 0.0);
            context.ContainerView?.SelectionState?.Select(_conic.Point2);

            CurrentState = State.Point1;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartPointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint1Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _conic.Point1.X = x;
            _conic.Point1.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePoint2Internal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _conic.Point1.X = x;
            _conic.Point1.Y = y;
            _conic.Point2.X = x;
            _conic.Point2.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));

            CurrentState = State.StartPoint;

            if (_conic != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_conic);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_conic);
                context.ContainerView?.SelectionState?.Deselect(_conic.StartPoint);
                context.ContainerView?.SelectionState?.Deselect(_conic.Point1);
                context.ContainerView?.SelectionState?.Deselect(_conic.Point2);
                _conic = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        StartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point1:
                    {
                        Point1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        Point2Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Point1:
                case State.Point2:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        MoveStartPointInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point1:
                    {
                        MovePoint1Internal(context, x, y, modifier);
                    }
                    break;
                case State.Point2:
                    {
                        MovePoint2Internal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class RectangleTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private RectangleToolSettings _settings;
        private RectangleShape _rectangle = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Rectangle";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public RectangleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _rectangle = new RectangleShape()
            {
                Points = new ObservableCollection<PointShape>(),
                TopLeft = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                BottomRight = context.ContainerView?.GetNextPoint(x, y, false, 0.0),
                Style = context.ContainerView?.CurrentStyle
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_rectangle);
            context.ContainerView?.SelectionState?.Select(_rectangle.TopLeft);
            context.ContainerView?.SelectionState?.Select(_rectangle.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_rectangle);
            context.ContainerView?.SelectionState?.Deselect(_rectangle.BottomRight);
            _rectangle.BottomRight = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _rectangle.BottomRight.Y = y;
            context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_rectangle.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_rectangle);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _rectangle = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            Filters?.ForEach(f => f.Clear(context));

            if (_rectangle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_rectangle);
                context.ContainerView?.SelectionState?.Deselect(_rectangle.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_rectangle.BottomRight);
                _rectangle = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        BottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.BottomRight:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        MoveBottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [DataContract(IsReference = true)]
    public class ScribbleToolSettings : Settings
    {
        private bool _simplify;
        private double _epsilon;
        private PathFillRule _fillRule;
        private bool _isFilled;
        private bool _isClosed;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathFillRule FillRule
        {
            get => _fillRule;
            set => Update(ref _fillRule, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class ScribbleTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private ScribbleToolSettings _settings;
        private PathShape _path = null;
        private FigureShape _figure = null;
        private PointShape _previousPoint = null;
        private PointShape _nextPoint = null;

        public enum State
        {
            Start,
            Points
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.Start;

        [IgnoreDataMember]
        public string Title => "Scribble";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ScribbleToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void StartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _path = new PathShape()
            {
                Points = new ObservableCollection<PointShape>(),
                Figures = new ObservableCollection<FigureShape>(),
                FillRule = Settings.FillRule,
                Text = new Text(),
                Style = context.ContainerView?.CurrentStyle
            };

            _figure = new FigureShape()
            {
                Shapes = new ObservableCollection<BaseShape>(),
                IsFilled = Settings.IsFilled,
                IsClosed = Settings.IsClosed
            };

            _path.Figures.Add(_figure);

            _previousPoint = new PointShape(x, y, context.ContainerView?.PointTemplate);

            context.ContainerView?.WorkingContainer.Shapes.Add(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Points;
        }

        private void PointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.Start;

            if (Settings?.Simplify ?? true)
            {
                var points = new List<PointShape>();
                _path.GetPoints(points);
                var distinct = points.Distinct().ToList();
                IList<Vector2> vectors = distinct.Select(p => new Vector2((float)p.X, (float)p.Y)).ToList();
                int count = vectors.Count;
                RDP rdp = new RDP();
                BitArray accepted = rdp.DouglasPeucker(vectors, 0, count - 1, Settings?.Epsilon ?? 1.0);
                int removed = 0;
                for (int i = 0; i <= count - 1; ++i)
                {
                    if (!accepted[i])
                    {
                        distinct.RemoveAt(i - removed);
                        ++removed;
                    }
                }

                _figure.Shapes.Clear();
                _figure.MarkAsDirty(true);

                if (distinct.Count >= 2)
                {
                    for (int i = 0; i < distinct.Count - 1; i++)
                    {
                        var line = new LineShape()
                        {
                            Points = new ObservableCollection<PointShape>(),
                            StartPoint = distinct[i],
                            Point = distinct[i + 1],
                            Style = context.ContainerView?.CurrentStyle
                        };
                        _figure.Shapes.Add(line);
                    }
                }
            }

            context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);

            if (_path.Validate(true) == true)
            {
                context.ContainerView?.CurrentContainer.Shapes.Add(_path);
                context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            }

            _path = null;
            _figure = null;
            _previousPoint = null;
            _nextPoint = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveStartInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MovePointsInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _nextPoint = new PointShape(x, y, context.ContainerView?.PointTemplate);

            var line = new LineShape()
            {
                Points = new ObservableCollection<PointShape>(),
                StartPoint = _previousPoint,
                Point = _nextPoint,
                Style = context.ContainerView?.CurrentStyle
            };

            _figure.Shapes.Add(line);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);

            _previousPoint = _nextPoint;
            _nextPoint = null;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.Start;

            Filters?.ForEach(f => f.Clear(context));

            if (_path != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_path);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _path = null;
                _figure = null;
                _previousPoint = null;
                _nextPoint = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Start:
                    {
                        StartInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Points:
                    {
                        PointsInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Points:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Start:
                    {
                        MoveStartInternal(context, x, y, modifier);
                    }
                    break;
                case State.Points:
                    {
                        MovePointsInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }

    [Flags]
    public enum SelectionMode
    {
        None = 0,
        Point = 1,
        Shape = 2,
        All = Point | Shape
    }

    [Flags]
    public enum SelectionTargets
    {
        None = 0,
        Shapes = 1,
        Guides = 2,
        All = Shapes | Guides
    }

    [DataContract(IsReference = true)]
    public class SelectionToolSettings : Settings
    {
        private SelectionMode _mode;
        private SelectionTargets _targets;
        private Modifier _selectionModifier;
        private Modifier _connectionModifier;
        private ShapeStyle _selectionStyle;
        private bool _clearSelectionOnClean;
        private double _hitTestRadius;
        private bool _connectPoints;
        private double _connectTestRadius;
        private bool _disconnectPoints;
        private double _disconnectTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionTargets Targets
        {
            get => _targets;
            set => Update(ref _targets, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Modifier SelectionModifier
        {
            get => _selectionModifier;
            set => Update(ref _selectionModifier, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Modifier ConnectionModifier
        {
            get => _connectionModifier;
            set => Update(ref _connectionModifier, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShapeStyle SelectionStyle
        {
            get => _selectionStyle;
            set => Update(ref _selectionStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ClearSelectionOnClean
        {
            get => _clearSelectionOnClean;
            set => Update(ref _clearSelectionOnClean, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ConnectTestRadius
        {
            get => _connectTestRadius;
            set => Update(ref _connectTestRadius, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool DisconnectPoints
        {
            get => _disconnectPoints;
            set => Update(ref _disconnectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double DisconnectTestRadius
        {
            get => _disconnectTestRadius;
            set => Update(ref _disconnectTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class SelectionState : ViewModelBase, ISelectionState, ICopyable
    {
        private BaseShape _hovered;
        private BaseShape _selected;
        private ISet<BaseShape> _shapes;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BaseShape Hovered
        {
            get => _hovered;
            set => Update(ref _hovered, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BaseShape Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ISet<BaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        public virtual object Copy(Dictionary<object, object> shared)
        {
            var copy = new SelectionState()
            {
                Name = this.Name,
                Hovered = this.Hovered,
                Selected = this.Selected,
                Shapes = new HashSet<BaseShape>()
            };

            foreach (var shape in this.Shapes)
            {
                copy.Shapes.Add(shape);
            }

            return copy;
        }

        public void Hover(BaseShape shape)
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

        public bool IsSelected(BaseShape shape)
        {
            if (shape != null && _shapes.Contains(shape))
            {
                return true;
            }
            return false;
        }

        public void Select(BaseShape shape)
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

        public void Deselect(BaseShape shape)
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

    [DataContract(IsReference = true)]
    public class SelectionTool : ViewModelBase, ITool, ISelection
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private SelectionToolSettings _settings;
        private RectangleShape _rectangle;
        private double _originX;
        private double _originY;
        private double _previousX;
        private double _previousY;
        private IList<BaseShape> _shapesToCopy = null;
        private bool _disconnected = false;

        public enum State
        {
            None,
            Selection,
            Move
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.None;

        [IgnoreDataMember]
        public string Title => "Selection";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public SelectionToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void LeftDownNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _disconnected = false;

            _originX = x;
            _originY = y;
            _previousX = x;
            _previousY = y;

            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref _originX, ref _originY));

            _previousX = _originX;
            _previousY = _originY;

            context.ContainerView?.SelectionState?.Dehover();

            var selected = TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0,
                modifier);
            if (selected == true)
            {
                context.ContainerView?.InputService?.Capture?.Invoke();

                CurrentState = State.Move;
            }
            else
            {
                if (!modifier.HasFlag(Settings?.SelectionModifier ?? Modifier.Control))
                {
                    context.ContainerView?.SelectionState?.Clear();
                }

                if (_rectangle == null)
                {
                    _rectangle = new RectangleShape()
                    {
                        Points = new ObservableCollection<PointShape>(),
                        TopLeft = new PointShape(),
                        BottomRight = new PointShape()
                    };
                }

                _rectangle.TopLeft.X = x;
                _rectangle.TopLeft.Y = y;
                _rectangle.BottomRight.X = x;
                _rectangle.BottomRight.Y = y;
                _rectangle.Style = Settings?.SelectionStyle;
                context.ContainerView?.WorkingContainer.Shapes.Add(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);

                context.ContainerView?.InputService?.Capture?.Invoke();
                context.ContainerView?.InputService?.Redraw?.Invoke();

                CurrentState = State.Selection;
            }
        }

        private void LeftDownSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            CurrentState = State.None;

            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.SelectionState?.Dehover();

            TryToSelect(
                context,
                Settings?.Mode ?? SelectionMode.Shape,
                Settings?.Targets ?? SelectionTargets.Shapes,
                Settings?.SelectionModifier ?? Modifier.Control,
                _rectangle.ToRect2(),
                Settings?.HitTestRadius ?? 7.0,
                modifier);

            context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            _rectangle = null;

            CurrentState = State.None;

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void LeftUpMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void RightDownMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.None;
        }

        private void MoveNoneInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null && !(context.ContainerView.SelectionState.Hovered == null && context.ContainerView.SelectionState.Shapes.Count > 0))
            {
                lock (context.ContainerView.SelectionState.Shapes)
                {
                    var previous = context.ContainerView?.SelectionState?.Hovered;
                    var target = new Point2(x, y);
                    var shape = TryToHover(
                        context,
                        Settings?.Mode ?? SelectionMode.Shape,
                        Settings?.Targets ?? SelectionTargets.Shapes,
                        target,
                        Settings?.HitTestRadius ?? 7.0);
                    if (shape != null)
                    {
                        if (shape != previous)
                        {
                            context.ContainerView?.SelectionState?.Dehover();
                            context.ContainerView?.SelectionState?.Hover(shape);
                            context.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                    else
                    {
                        if (previous != null)
                        {
                            context.ContainerView?.SelectionState?.Dehover();
                            context.ContainerView?.InputService?.Redraw?.Invoke();
                        }
                    }
                }
            }
        }

        private void MoveSelectionInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            _rectangle.BottomRight.X = x;
            _rectangle.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveMoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            double dx = x - _previousX;
            double dy = y - _previousY;

            _previousX = x;
            _previousY = y;

            if (context.ContainerView?.SelectionState != null)
            {
                if (context.ContainerView.SelectionState.Shapes.Count == 1)
                {
                    var shape = context.ContainerView.SelectionState.Shapes.FirstOrDefault();

                    if (shape is PointShape source)
                    {
                        if (Settings.ConnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            Connect(context, source);
                        }

                        if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            if (_disconnected == false)
                            {
                                double treshold = Settings.DisconnectTestRadius;
                                double tx = Math.Abs(_originX - source.X);
                                double ty = Math.Abs(_originY - source.Y);
                                if (tx > treshold || ty > treshold)
                                {
                                    Disconnect(context, source);
                                }
                            }
                        }
                    }

                    shape.Move(context.ContainerView.SelectionState, dx, dy);
                }
                else
                {
                    foreach (var shape in context.ContainerView.SelectionState.Shapes.ToList())
                    {
                        if (Settings.DisconnectPoints && modifier.HasFlag(Settings?.ConnectionModifier ?? Modifier.Shift))
                        {
                            if (!(shape is PointShape) && _disconnected == false)
                            {
                                Disconnect(context, shape);
                            }
                        }
                    }

                    foreach (var shape in context.ContainerView.SelectionState.Shapes.ToList())
                    {
                        shape.Move(context.ContainerView.SelectionState, dx, dy);
                    }
                }

                context.ContainerView?.InputService?.Redraw?.Invoke();
            }
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.None;

            _disconnected = false;

            context.ContainerView?.SelectionState?.Dehover();

            if (_rectangle != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_rectangle);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                _rectangle = null;
            }

            if (Settings?.ClearSelectionOnClean == true)
            {
                context.ContainerView?.SelectionState?.Dehover();
                context.ContainerView?.SelectionState?.Clear();
            }

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        LeftDownNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        LeftDownSelectionInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Selection:
                    {
                        LeftUpSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        LeftUpMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.Selection:
                    {
                        this.Clean(context);
                    }
                    break;
                case State.Move:
                    {
                        RightDownMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.None:
                    {
                        MoveNoneInternal(context, x, y, modifier);
                    }
                    break;
                case State.Selection:
                    {
                        MoveSelectionInternal(context, x, y, modifier);
                    }
                    break;
                case State.Move:
                    {
                        MoveMoveInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }

        public void Cut(IToolContext context)
        {
            Copy(context);
            Delete(context);
        }

        public void Copy(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState.Shapes)
                {
                    _shapesToCopy = context.ContainerView.SelectionState.Shapes.ToList();
                }
            }
        }

        public void Paste(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                if (_shapesToCopy != null)
                {
                    lock (context.ContainerView.SelectionState.Shapes)
                    {
                        context.ContainerView?.SelectionState?.Dehover();
                        context.ContainerView?.SelectionState?.Clear();

                        Copy(context.ContainerView?.CurrentContainer, _shapesToCopy, context.ContainerView.SelectionState);

                        context.ContainerView?.InputService?.Redraw?.Invoke();

                        this.CurrentState = State.None;
                    }
                }
            }
        }

        public void Delete(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState.Shapes)
                {
                    Delete(context.ContainerView?.CurrentContainer, context.ContainerView.SelectionState);

                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Group(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();

                    var shapes = context.ContainerView.SelectionState.Shapes.ToList();

                    Delete(context);

                    var group = new GroupShape()
                    {
                        Title = "Group",
                        Points = new ObservableCollection<PointShape>(),
                        Shapes = new ObservableCollection<BaseShape>()
                    };

                    foreach (var shape in shapes)
                    {
                        if (!(shape is PointShape))
                        {
                            group.Shapes.Add(shape);
                        }
                    }

                    group.Select(context.ContainerView.SelectionState);
                    context.ContainerView?.CurrentContainer.Shapes.Add(group);
                    context.ContainerView?.CurrentContainer.MarkAsDirty(true);

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void SelectAll(IToolContext context)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                lock (context.ContainerView.SelectionState.Shapes)
                {
                    context.ContainerView?.SelectionState?.Dehover();
                    context.ContainerView?.SelectionState?.Clear();

                    foreach (var shape in context.ContainerView?.CurrentContainer.Shapes)
                    {
                        shape.Select(context.ContainerView.SelectionState);
                    }

                    context.ContainerView?.InputService?.Redraw?.Invoke();

                    this.CurrentState = State.None;
                }
            }
        }

        public void Connect(IToolContext context, PointShape point)
        {
            var target = context.ContainerView?.HitTest.TryToGetPoint(
                context.ContainerView?.CurrentContainer.Shapes,
                new Point2(point.X, point.Y),
                Settings?.ConnectTestRadius ?? 7.0,
                point);
            if (target != point)
            {
                foreach (var item in context.ContainerView?.CurrentContainer.Shapes)
                {
                    if (item is ConnectableShape connectable)
                    {
                        if (connectable.Connect(point, target))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, PointShape point)
        {
            foreach (var shape in context.ContainerView?.CurrentContainer.Shapes)
            {
                if (shape is ConnectableShape connectable)
                {
                    if (connectable.Disconnect(point, out var copy))
                    {
                        if (copy != null)
                        {
                            point.X = _originX;
                            point.Y = _originY;
                            context.ContainerView?.SelectionState?.Deselect(point);
                            context.ContainerView?.SelectionState?.Select(copy);
                            _disconnected = true;
                        }
                        break;
                    }
                }
            }
        }

        public void Disconnect(IToolContext context, BaseShape shape)
        {
            if (shape is ConnectableShape connectable)
            {
                if (context.ContainerView?.SelectionState != null)
                {
                    connectable.Deselect(context.ContainerView.SelectionState);
                }
                _disconnected = connectable.Disconnect();
                if (context.ContainerView?.SelectionState != null)
                {
                    connectable.Select(context.ContainerView.SelectionState);
                }
            }
        }

        internal Dictionary<object, object> GetPointsCopyDict(IEnumerable<BaseShape> shapes)
        {
            var copy = new Dictionary<object, object>();

            var points = new List<PointShape>();

            foreach (var shape in shapes)
            {
                shape.GetPoints(points);
            }

            var distinct = points.Distinct();

            foreach (var point in distinct)
            {
                copy[point] = point.Copy(null);
            }

            return copy;
        }

        internal void Copy(ICanvasContainer container, IEnumerable<BaseShape> shapes, ISelectionState selectionState)
        {
            var shared = GetPointsCopyDict(shapes);

            foreach (var shape in shapes)
            {
                if (shape is ICopyable copyable)
                {
                    var copy = (BaseShape)copyable.Copy(shared);
                    if (copy != null && !(copy is PointShape))
                    {
                        copy.Select(selectionState);
                        container.Shapes.Add(copy);
                    }
                }
            }
        }

        internal void Delete(ICanvasContainer container, ISelectionState selectionState)
        {
            // TODO: Very slow when using Contains.
            //var paths = container.Shapes.OfType<PathShape>().ToList().AsReadOnly();
            //var groups = container.Shapes.OfType<GroupShape>().ToList().AsReadOnly();
            //var connectables = container.Shapes.OfType<ConnectableShape>().ToList().AsReadOnly();

            var shapesHash = new HashSet<BaseShape>(container.Shapes);

            foreach (var shape in selectionState.Shapes)
            {
                if (shapesHash.Contains(shape))
                {
                    container.Shapes.Remove(shape);
                    container.MarkAsDirty(true);
                }
                /*
                else
                {
                    if (shape is PointShape point)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(connectables, point);
                    }

                    if (paths.Count > 0)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(container, paths, shape);
                    }

                    if (groups.Count > 0)
                    {
                        // TODO: Very slow when using Contains.
                        TryToDelete(container, groups, shape);
                    }
                }
                */
            }
        }

        internal bool TryToDelete(IReadOnlyList<ConnectableShape> connectables, PointShape point)
        {
            foreach (var connectable in connectables)
            {
                if (connectable.Points.Contains(point))
                {
                    connectable.Points.Remove(point);
                    connectable.MarkAsDirty(true);

                    return true;
                }
            }

            return false;
        }

        internal bool TryToDelete(ICanvasContainer container, IReadOnlyList<PathShape> paths, BaseShape shape)
        {
            foreach (var path in paths)
            {
                foreach (var figure in path.Figures)
                {
                    if (figure.Shapes.Contains(shape))
                    {
                        figure.Shapes.Remove(shape);
                        figure.MarkAsDirty(true);

                        if (figure.Shapes.Count <= 0)
                        {
                            path.Figures.Remove(figure);
                            path.MarkAsDirty(true);

                            if (path.Figures.Count <= 0)
                            {
                                container.Shapes.Remove(path);
                                container.MarkAsDirty(true);
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        internal bool TryToDelete(ICanvasContainer container, IReadOnlyList<GroupShape> groups, BaseShape shape)
        {
            foreach (var group in groups)
            {
                if (group.Shapes.Contains(shape))
                {
                    group.Shapes.Remove(shape);
                    group.MarkAsDirty(true);

                    if (group.Shapes.Count <= 0)
                    {
                        container.Shapes.Remove(group);
                        container.MarkAsDirty(true);
                    }

                    return true;
                }
            }

            return false;
        }

        internal BaseShape TryToHover(IToolContext context, SelectionMode mode, SelectionTargets targets, Point2 target, double radius)
        {
            var shapePoint =
                mode.HasFlag(SelectionMode.Point)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.ContainerView?.HitTest?.TryToGetPoint(context.ContainerView?.CurrentContainer.Shapes, target, radius, null) : null;

            var shape =
                mode.HasFlag(SelectionMode.Shape)
                && targets.HasFlag(SelectionTargets.Shapes) ?
                context.ContainerView?.HitTest?.TryToGetShape(context.ContainerView?.CurrentContainer.Shapes, target, radius) : null;

            if (shapePoint != null || shape != null)
            {
                if (shapePoint != null)
                {
                    return shapePoint;
                }
                else if (shape != null)
                {
                    return shape;
                }
            }

            return null;
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Point2 point, double radius, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                var shapePoint =
                    mode.HasFlag(SelectionMode.Point)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.ContainerView?.HitTest?.TryToGetPoint(context.ContainerView?.CurrentContainer.Shapes, point, radius, null) : null;

                var shape =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.ContainerView?.HitTest?.TryToGetShape(context.ContainerView?.CurrentContainer.Shapes, point, radius) : null;

                if (shapePoint != null || shape != null)
                {
                    bool haveNewSelection =
                        (shapePoint != null && !context.ContainerView.SelectionState.IsSelected(shapePoint))
                        || (shape != null && !context.ContainerView.SelectionState.IsSelected(shape));

                    if (context.ContainerView.SelectionState.Shapes.Count >= 1
                        && !haveNewSelection
                        && !modifier.HasFlag(selectionModifier))
                    {
                        return true;
                    }
                    else
                    {
                        if (shapePoint != null)
                        {
                            if (modifier.HasFlag(selectionModifier))
                            {
                                if (context.ContainerView.SelectionState.IsSelected(shapePoint))
                                {
                                    shapePoint.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shapePoint.Select(context.ContainerView.SelectionState);
                                }
                                return context.ContainerView.SelectionState.Shapes.Count > 0;
                            }
                            else
                            {
                                context.ContainerView.SelectionState.Clear();
                                shapePoint.Select(context.ContainerView.SelectionState);
                                return true;
                            }
                        }
                        else if (shape != null)
                        {
                            if (modifier.HasFlag(selectionModifier))
                            {
                                if (context.ContainerView.SelectionState.IsSelected(shape))
                                {
                                    shape.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.ContainerView.SelectionState);
                                }
                                return context.ContainerView.SelectionState.Shapes.Count > 0;
                            }
                            else
                            {
                                context.ContainerView.SelectionState.Clear();
                                shape.Select(context.ContainerView.SelectionState);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        internal bool TryToSelect(IToolContext context, SelectionMode mode, SelectionTargets targets, Modifier selectionModifier, Rect2 rect, double radius, Modifier modifier)
        {
            if (context.ContainerView?.SelectionState != null)
            {
                var shapes =
                    mode.HasFlag(SelectionMode.Shape)
                    && targets.HasFlag(SelectionTargets.Shapes) ?
                    context.ContainerView?.HitTest?.TryToGetShapes(context.ContainerView?.CurrentContainer.Shapes, rect, radius) : null;

                if (shapes != null)
                {
                    if (shapes != null)
                    {
                        if (modifier.HasFlag(selectionModifier))
                        {
                            foreach (var shape in shapes)
                            {
                                if (context.ContainerView.SelectionState.IsSelected(shape))
                                {
                                    shape.Deselect(context.ContainerView.SelectionState);
                                }
                                else
                                {
                                    shape.Select(context.ContainerView.SelectionState);
                                }
                            }
                            return context.ContainerView.SelectionState.Shapes.Count > 0;
                        }
                        else
                        {
                            context.ContainerView.SelectionState.Clear();
                            foreach (var shape in shapes)
                            {
                                shape.Select(context.ContainerView.SelectionState);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    [DataContract(IsReference = true)]
    public class TextToolSettings : Settings
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }
    }

    [DataContract(IsReference = true)]
    public class TextTool : ViewModelBase, ITool
    {
        private IList<PointIntersection> _intersections;
        private IList<PointFilter> _filters;
        private TextToolSettings _settings;
        private TextShape _text = null;

        public enum State
        {
            TopLeft,
            BottomRight
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.TopLeft;

        [IgnoreDataMember]
        public string Title => "Text";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointIntersection> Intersections
        {
            get => _intersections;
            set => Update(ref _intersections, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IList<PointFilter> Filters
        {
            get => _filters;
            set => Update(ref _filters, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TextToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public override void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _text = new TextShape()
            {
                Points = new ObservableCollection<PointShape>(),
                TopLeft = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0),
                BottomRight = context.ContainerView?.GetNextPoint(x, y, false, 0.0),
                Text = new Text("Text"),
                Style = context.ContainerView?.CurrentStyle,
            };
            context.ContainerView?.WorkingContainer.Shapes.Add(_text);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Select(_text);
            context.ContainerView?.SelectionState?.Select(_text.TopLeft);
            context.ContainerView?.SelectionState?.Select(_text.BottomRight);

            context.ContainerView?.InputService?.Capture?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.BottomRight;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.Any(f => f.Process(context, ref x, ref y));

            CurrentState = State.TopLeft;

            context.ContainerView?.SelectionState?.Deselect(_text);
            context.ContainerView?.SelectionState?.Deselect(_text.BottomRight);
            _text.BottomRight = context.ContainerView?.GetNextPoint(x, y, Settings?.ConnectPoints ?? false, Settings?.HitTestRadius ?? 7.0);
            _text.BottomRight.Y = y;
            context.ContainerView?.WorkingContainer.Shapes.Remove(_text);
            context.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.ContainerView?.SelectionState?.Deselect(_text.TopLeft);
            context.ContainerView?.CurrentContainer.Shapes.Add(_text);
            context.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _text = null;

            Filters?.ForEach(f => f.Clear(context));

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            _text.BottomRight.X = x;
            _text.BottomRight.Y = y;

            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.TopLeft;

            Filters?.ForEach(f => f.Clear(context));

            if (_text != null)
            {
                context.ContainerView?.WorkingContainer.Shapes.Remove(_text);
                context.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.ContainerView?.SelectionState?.Deselect(_text);
                context.ContainerView?.SelectionState?.Deselect(_text.TopLeft);
                context.ContainerView?.SelectionState?.Deselect(_text.BottomRight);
                _text = null;
            }

            context.ContainerView?.InputService?.Release?.Invoke();
            context.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        BottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.BottomRight:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.BottomRight:
                    {
                        MoveBottomRightInternal(context, x, y, modifier);
                    }
                    break;
            }
        }

        public void Clean(IToolContext context)
        {
            CleanInternal(context);
        }
    }
}
