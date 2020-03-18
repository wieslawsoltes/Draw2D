// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class PointTool : BaseTool, ITool
    {
        private PointToolSettings _settings;

        [IgnoreDataMember]
        public new string Title => "Point";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PointToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void PointInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            var shape = context.HitTest?.TryToGetShape(
                context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes,
                new Point2(x, y),
                radius,
                scale,
                modifier);
            if (shape != null && (Settings?.ConnectPoints ?? false))
            {
                if (shape is IConnectable connectable)
                {
                    var point = new PointShape(x, y, context?.DocumentContainer?.PointTemplate);
                    point.Owner = connectable;
                    connectable.Points.Add(point);
                    context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                    context.DocumentContainer?.ContainerView?.SelectionState?.Select(point);
                    context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
                }
            }
        }

        private void MoveInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            FiltersClear(context);
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
}
