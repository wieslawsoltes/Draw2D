// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Spatial;

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
            double width = 1;
            var matrix = Matrix.MakeFrom(Matrix2.Scale(15, 15));
            return new Path2DLineEffect(width, matrix) { Title = "HatchHorizontalLines" };
        }

        public static IPathEffect MakeHatchVerticalLines()
        {
            double width = 1;
            var matrix = Matrix.MakeFrom(Matrix2.Rotation(90 * Vector2.DegreesToRadians) * Matrix2.Scale(15, 15));
            return new Path2DLineEffect(width, matrix) { Title = "HatchVerticalLines" };
        }

        public static IPathEffect MakeHatchDiagonalLines()
        {
            double width = 1;
            var matrix = Matrix.MakeFrom(Matrix2.Scale(15, 15) * Matrix2.Rotation(45 * Vector2.DegreesToRadians));
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
