// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathCornerEffect : ViewModelBase, IPathEffect
    {
        private double _radius;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Radius
        {
            get => _radius;
            set => Update(ref _radius, value);
        }

        public PathCornerEffect()
        {
        }

        public PathCornerEffect(double radius)
        {
            this.Radius = radius;
        }

        public static IPathEffect MakeCorner()
        {
            return new PathCornerEffect(5.0) { Title = "Corner" };
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathCornerEffect()
            {
                Title = this.Title,
                Radius = this.Radius
            };
        }
    }
}
