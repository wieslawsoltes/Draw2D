using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Decorators;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Containers;

[DataContract(IsReference = true)]
public class CanvasContainer : GroupShape, ICanvasContainer
{
    internal static new IBounds s_bounds = new ContainerBounds();
    internal static new IShapeDecorator s_decorator = new ContainerDecorator();

    [IgnoreDataMember]
    public override IBounds Bounds { get; } = s_bounds;

    [IgnoreDataMember]
    public override IShapeDecorator Decorator { get; } = s_decorator;

    public CanvasContainer()
    {
    }

    public override object Copy(Dictionary<object, object> shared)
    {
        var copy = new CanvasContainer()
        {
            Name = this.Name,
            Title = this.Title + "_copy",
            Points = new ObservableCollection<IPointShape>(),
            Shapes = new ObservableCollection<IBaseShape>(),
            StyleId = this.StyleId
        };

        if (shared != null)
        {
            foreach (var point in this.Points)
            {
                copy.Points.Add((IPointShape)shared[point]);
            }

            foreach (var shape in this.Shapes)
            {
                if (shape is ICopyable copyable)
                {
                    copy.Shapes.Add((IBaseShape)(copyable.Copy(shared)));
                }
            }

            shared[this] = copy;
            shared[copy] = this;
        }

        return copy;
    }
}