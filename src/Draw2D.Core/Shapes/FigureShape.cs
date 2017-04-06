// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;

namespace Draw2D.Core.Shapes
{
    public class FigureShape : GroupShape, ICopyable<FigureShape>
    {
        private bool _isFilled;
        private bool _isClosed;

        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        public FigureShape()
            : base()
        {
        }

        public FigureShape(ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Shapes = shapes;
        }

        public FigureShape(string name)
            : this()
        {
            this.Name = name;
        }

        public FigureShape(string name, ObservableCollection<ShapeObject> shapes)
            : base()
        {
            this.Name = name;
            this.Shapes = shapes;
        }

        public new FigureShape Copy()
        {
            return new FigureShape()
            {
                Style = this.Style,
                Transform = this.Transform
            };
        }
    }
}
