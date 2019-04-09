// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Tools
{
    public class PointToolSettings : SettingsBase
    {
        private bool _connectPoints;
        private double _hitTestRadius;

        public bool ConnectPoints
        {
            get => _connectPoints;
            set => Update(ref _connectPoints, value);
        }

        public double HitTestRadius
        {
            get => _hitTestRadius;
            set => Update(ref _hitTestRadius, value);
        }
    }

    public class PointTool : ToolBase
    {
        public override string Title => "Point";

        public PointToolSettings Settings { get; set; }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            var point = new PointShape(x, y, context.PointShape);

            var shape = context.HitTest?.TryToGetShape(
                context.CurrentContainer.Shapes,
                new Point2(x, y),
                Settings?.HitTestRadius ?? 7.0);
            if (shape != null && (Settings?.ConnectPoints ?? false))
            {
                if (shape is ConnectableShape connectable)
                {
                    connectable.Points.Add(point);
                    context.Renderer.Selected.Add(point);
                    context.Invalidate?.Invoke();
                }
            }
            //else
            //{
            //    context.CurrentContainer.Shapes.Add(point);
            //    context.Invalidate?.Invoke();
            //}
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            Filters?.ForEach(f => f.Clear(context));
            Filters?.Any(f => f.Process(context, ref x, ref y));

            context.Invalidate?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            Filters?.ForEach(f => f.Clear(context));
        }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            PointInternal(context, x, y, modifier);
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            MoveInternal(context, x, y, modifier);
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CleanInternal(context);
        }
    }
}
