using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Tools;

namespace Draw2D.ViewModels.Filters
{
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
                    x = (double)SnapGrid((decimal)x, (decimal)Settings.GridSizeX);
                    haveSnapToGrid = true;
                }

                if (Settings.Mode.HasFlag(GridSnapMode.Vertical))
                {
                    y = (double)SnapGrid((decimal)y, (decimal)Settings.GridSizeY);
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
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = new PointShape(0, y, null),
                Point = new PointShape(context.DocumentContainer?.ContainerView?.Width ?? 0, y, null),
                StyleId = Settings.GuideStyle
            };
            horizontal.StartPoint.Owner = horizontal;
            horizontal.Point.Owner = horizontal;

            var vertical = new LineShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = new PointShape(x, 0, null),
                Point = new PointShape(x, context.DocumentContainer?.ContainerView?.Height ?? 0, null),
                StyleId = Settings.GuideStyle
            };
            vertical.StartPoint.Owner = vertical;
            vertical.Point.Owner = vertical;

            Guides.Add(horizontal);
            Guides.Add(vertical);

            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(horizontal);
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(vertical);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
        }

        public static decimal SnapGrid(decimal value, decimal size)
        {
            decimal r = value % size;
            return r >= size / 2.0m ? value + size - r : value - r;
        }
    }
}
