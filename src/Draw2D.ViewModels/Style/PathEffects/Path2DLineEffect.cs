// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class Path2DLineEffect : ViewModelBase, IPathEffect
    {
        private double _width;
        private Matrix _matrix;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Matrix Matrix
        {
            get => _matrix;
            set => Update(ref _matrix, value);
        }

        public Path2DLineEffect()
        {
        }

        public Path2DLineEffect(double width, Matrix matrix)
        {
            this.Width = width;
            this.Matrix = matrix;
        }

        public static IPathEffect MakeHatchHorizontalLines()
        {
            // TODO:
            double width = 3;
            var matrix = Matrix.MakeIdentity();
            matrix.ScaleX = 6;
            matrix.ScaleY = 6;
            return new Path2DLineEffect(width, matrix) { Title = "HatchHorizontalLines" };
        }

        public static IPathEffect MakeHatchVerticalLines()
        {
            // TODO:
            double width = 6;
            var matrix = Matrix.MakeIdentity();
            return new Path2DLineEffect(width, matrix) { Title = "HatchVerticalLines" };
        }

        public static IPathEffect MakeHatchDiagonalLines()
        {
            // TODO:
            double width = 12;
            var matrix = Matrix.MakeIdentity();
            return new Path2DLineEffect(width, matrix) { Title = "HatchDiagonalLines" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Path2DLineEffect()
            {
                Title = this.Title,
                Width = this.Width,
                Matrix = (Matrix)this.Matrix.Copy(shared)
            };
        }
    }
}
