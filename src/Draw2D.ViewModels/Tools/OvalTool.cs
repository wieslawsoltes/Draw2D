// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Draw2D.Input;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class OvalTool : BaseTool, ITool
    {
        private OvalToolSettings _settings;
        private OvalShape _oval = null;

        public enum State
        {
            StartPoint,
            Point
        }

        [IgnoreDataMember]
        public State CurrentState { get; set; } = State.StartPoint;

        [IgnoreDataMember]
        public new string Title => "oval";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public OvalToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        private void TopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape startPoint = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);
            IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, false, 0.0, 1.0, modifier);

            _oval = new OvalShape()
            {
                Points = new ObservableCollection<IPointShape>(),
                StartPoint = startPoint,
                Point = point,
                Text = new Text(),
                StyleId = context.DocumentContainer?.StyleLibrary?.CurrentItem?.Title
            };
            _oval.Owner = context.DocumentContainer?.ContainerView?.WorkingContainer;
            if (_oval.StartPoint.Owner == null)
            {
                _oval.StartPoint.Owner = _oval;
            }
            if (_oval.Point.Owner == null)
            {
                _oval.Point.Owner = _oval;
            }
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Add(_oval);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_oval);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_oval.StartPoint);
            context.DocumentContainer?.ContainerView?.SelectionState?.Select(_oval.Point);

            context.DocumentContainer?.ContainerView?.InputService?.Capture?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();

            CurrentState = State.Point;
        }

        private void BottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersProcess(context, ref x, ref y);

            CurrentState = State.StartPoint;

            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_oval.Point);

            var radius = Settings?.HitTestRadius ?? 7.0;
            var scale = context.DocumentContainer?.ContainerView?.ZoomService?.ZoomServiceState?.ZoomX ?? 1.0;

            IPointShape point = context.DocumentContainer?.ContainerView?.GetNextPoint(context, x, y, Settings?.ConnectPoints ?? false, radius, scale, modifier);

            _oval.Point = point;
            if (_oval.Point.Owner == null)
            {
                _oval.Point.Owner = _oval;
            }
            context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_oval);
            context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_oval);
            context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_oval.StartPoint);
            _oval.Owner = context.DocumentContainer?.ContainerView?.CurrentContainer;
            context.DocumentContainer?.ContainerView?.CurrentContainer.Shapes.Add(_oval);
            context.DocumentContainer?.ContainerView?.CurrentContainer.MarkAsDirty(true);
            _oval = null;

            FiltersClear(context);

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveTopLeftInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void MoveBottomRightInternal(IToolContext context, double x, double y, Modifier modifier)
        {
            FiltersClear(context);
            FiltersProcess(context, ref x, ref y);

            _oval.Point.X = x;
            _oval.Point.Y = y;

            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        private void CleanInternal(IToolContext context)
        {
            CurrentState = State.StartPoint;

            FiltersClear(context);

            if (_oval != null)
            {
                context.DocumentContainer?.ContainerView?.WorkingContainer.Shapes.Remove(_oval);
                context.DocumentContainer?.ContainerView?.WorkingContainer.MarkAsDirty(true);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_oval);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_oval.StartPoint);
                context.DocumentContainer?.ContainerView?.SelectionState?.Deselect(_oval.Point);
                _oval = null;
            }

            context.DocumentContainer?.ContainerView?.InputService?.Release?.Invoke();
            context.DocumentContainer?.ContainerView?.InputService?.Redraw?.Invoke();
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            switch (CurrentState)
            {
                case State.StartPoint:
                    {
                        TopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
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
                        MoveTopLeftInternal(context, x, y, modifier);
                    }
                    break;
                case State.Point:
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
