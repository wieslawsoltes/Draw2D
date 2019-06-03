// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using Spatial;
using Spatial.ConvexHull;
using Spatial.DouglasPeucker;
using Spatial.Sat;

namespace Draw2D.ViewModels
{
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

    public interface INode
    {
        string Id { get; set; }
        string Name { get; set; }
        object Owner { get; set; }
    }

    public interface ICopyable
    {
        object Copy(Dictionary<object, object> shared);
    }

    public interface IShapeDecorator
    {
        void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale, DrawMode mode);
    }

    public interface IShapeRenderer : IDisposable
    {
        ISelectionState SelectionState { get; }
        void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale);
        void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale);
        void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale);
        void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale);
        void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale);
        void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale);
        void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale);
        void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale);
    }

    public interface IDrawable
    {
        IBounds Bounds { get; }
        IShapeDecorator Decorator { get; }
        string StyleId { get; set; }
        void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, DrawMode mode, object db, object r);
    }

    public interface IDirty
    {
        bool IsDirty { get; set; }
        void MarkAsDirty(bool isDirty);
        void Invalidate();
    }

    public interface IBaseShape : INode, ICopyable, IDirty, ISelectable, IDrawable
    {
        void GetPoints(IList<IPointShape> points);
    }

    public interface IPointShape : IBaseShape
    {
        double X { get; set; }
        double Y { get; set; }
        IBaseShape Template { get; set; }
    }

    public interface IConnectable
    {
        IList<IPointShape> Points { get; set; }
        bool Connect(IPointShape point, IPointShape target);
        bool Disconnect(IPointShape point, out IPointShape result);
        bool Disconnect();
    }

    public interface IHitTestable
    {
        IPointShape GetNextPoint(IToolContext context, double x, double y, bool connect, double radius);
    }

    public interface IBounds
    {
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IHitTest hitTest);
        IBaseShape Contains(IBaseShape shape, Point2 target, double radius, IHitTest hitTest);
        IBaseShape Overlaps(IBaseShape shape, Rect2 target, double radius, IHitTest hitTest);
    }

    public interface IHitTest
    {
        IPointShape TryToGetPoint(IEnumerable<IBaseShape> shapes, Point2 target, double radius, IPointShape exclude);
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius);
        IBaseShape TryToGetShape(IEnumerable<IBaseShape> shapes, Point2 target, double radius);
        ISet<IBaseShape> TryToGetShapes(IEnumerable<IBaseShape> shapes, Rect2 target, double radius);
    }

    public interface ISelectionState : INode, IDirty
    {
        IBaseShape Hovered { get; set; }
        IBaseShape Selected { get; set; }
        ISet<IBaseShape> Shapes { get; set; }
        void Hover(IBaseShape shape);
        void Dehover();
        bool IsSelected(IBaseShape shape);
        void Select(IBaseShape shape);
        void Deselect(IBaseShape shape);
        void Clear();
    }

    public interface ISelectable
    {
        void Move(ISelectionState selectionState, double dx, double dy);
        void Select(ISelectionState selectionState);
        void Deselect(ISelectionState selectionState);
    }
}

namespace Draw2D.ViewModels
{
    [DataContract(IsReference = true)]
    public abstract class ViewModelBase : INode, IDirty, INotifyPropertyChanged
    {
        private string _id = null;
        private string _name = null;
        private object _owner;

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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public object Owner
        {
            get => _owner;
            set => Update(ref _owner, value);
        }

        [IgnoreDataMember]
        public bool IsDirty { get; set; }

        public virtual void MarkAsDirty(bool value) => IsDirty = value;

        public virtual void Invalidate()
        {
            if (this.IsDirty)
            {
                this.IsDirty = false;
            }
        }

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
    public abstract class Settings : ViewModelBase
    {
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

        public object Copy(Dictionary<object, object> shared)
        {
            return new Text()
            {
                Value = this.Value
            };
        }
    }
}
