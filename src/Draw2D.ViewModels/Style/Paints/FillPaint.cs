// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class FillPaint : BasePaint, IFillPaint
    {
        public FillPaint()
        {
        }

        public FillPaint(
            ArgbColor color,
            bool isAntialias = true,
            IPaintEffects effects = null)
        {
            this.Color = color;
            this.IsAntialias = isAntialias;
            this.Effects = effects;
        }

        public override object Copy(Dictionary<object, object> shared)
        {
            return new FillPaint()
            {
                Name = this.Name,
                Title = this.Title,
                Color = (ArgbColor)(this.Color.Copy(shared)),
                IsAntialias = this.IsAntialias,
                Effects = (IPaintEffects)this.Effects.Copy(shared)
            };
        }
    }
}
