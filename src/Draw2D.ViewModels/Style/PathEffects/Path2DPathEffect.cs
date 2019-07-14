// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;
using Spatial;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class Path2DPathEffect : ViewModelBase, IPathEffect
    {
        private Matrix _matrix;
        private string _path;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Matrix Matrix
        {
            get => _matrix;
            set => Update(ref _matrix, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Path
        {
            get => _path;
            set => Update(ref _path, value);
        }

        public Path2DPathEffect()
        {
        }

        public Path2DPathEffect(Matrix matrix, string path)
        {
            this.Matrix = matrix;
            this.Path = path;
        }

        public static IPathEffect MakeTile()
        {
            var matrix = Matrix.MakeFrom(Matrix2.Scale(30, 30));
            var path = "M-15 -15L15 -15L15 15L-15 15L-15 -15Z";
            return new Path2DPathEffect(matrix, path) { Title = "2DPathTile" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new Path2DPathEffect()
            {
                Title = this.Title,
                Matrix = (Matrix)this.Matrix.Copy(shared),
                Path = this.Path
            };
        }
    }
}
