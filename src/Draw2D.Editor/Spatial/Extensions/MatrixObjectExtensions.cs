// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Renderer;
using Spatial;

namespace Draw2D.Core.Shapes
{
    public static class MatrixObjectExtensions
    {
        public static Matrix2 ToMatrix2(this MatrixObject matrix)
        {
            return new Matrix2(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }

        public static MatrixObject FromMatrix2(this Matrix2 matrix)
        {
            return new MatrixObject(
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.OffsetX, matrix.OffsetY);
        }
    }
}
