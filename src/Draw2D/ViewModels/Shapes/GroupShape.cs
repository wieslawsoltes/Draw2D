﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels.Shapes;

[DataContract(IsReference = true)]
public class GroupShape : BaseShape, ICanvasContainer
{
    internal static new IBounds s_bounds = new GroupBounds();
    internal static new IShapeDecorator s_decorator = new GroupDecorator();

    private string _title;
    private IList<IBaseShape> _shapes;

    [IgnoreDataMember]
    public override IBounds Bounds { get; } = s_bounds;

    [IgnoreDataMember]
    public override IShapeDecorator Decorator { get; } = s_decorator;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Title
    {
        get => _title;
        set => Update(ref _title, value);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<IBaseShape> Shapes
    {
        get => _shapes;
        set => Update(ref _shapes, value);
    }

    public GroupShape()
        : base()
    {
    }

    public GroupShape(IList<IBaseShape> shapes)
        : base()
    {
        this.Shapes = shapes;
    }

    public GroupShape(string title)
        : this()
    {
        this.Title = title;
    }

    public GroupShape(string title, IList<IBaseShape> shapes)
        : base()
    {
        this.Title = title;
        this.Shapes = shapes;
    }

    public override void GetPoints(IList<IPointShape> points)
    {
        base.GetPoints(points);

        foreach (var shape in Shapes)
        {
            shape.GetPoints(points);
        }
    }

    public override bool IsTreeDirty()
    {
        if (base.IsTreeDirty())
        {
            return true;
        }

        if (_shapes != null)
        {
            foreach (var shape in _shapes)
            {
                if (shape.IsTreeDirty())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public override void Invalidate()
    {
        foreach (var shape in Shapes)
        {
            shape.Invalidate();
        }

        base.Invalidate();
    }

    public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy, double scale, object db, object r)
    {
        if (Shapes != null)
        {
            foreach (var shape in Shapes)
            {
                shape.Draw(dc, renderer, dx, dy, scale, db, r);
            }
        }
    }

    public override void Move(ISelectionState selectionState, double dx, double dy)
    {
        var points = new List<IPointShape>();
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
            Title = this.Title + "_copy",
            Points = new ObservableCollection<IPointShape>(),
            Shapes = new ObservableCollection<IBaseShape>(),
            StyleId = this.StyleId,
            Effects = (IPaintEffects)this.Effects?.Copy(shared)
        };

        if (shared != null)
        {
            foreach (var point in this.Points)
            {
                var pointCopy = (IPointShape)shared[point];
                pointCopy.Owner = copy;
                copy.Points.Add(pointCopy);
            }

            foreach (var shape in this.Shapes)
            {
                if (shape is ICopyable copyable)
                {
                    var shapeCopy = (IBaseShape)(copyable.Copy(shared));
                    shapeCopy.Owner = copy;
                    copy.Shapes.Add(shapeCopy);
                }
            }

            shared[this] = copy;
            shared[copy] = this;
        }

        return copy;
    }
}