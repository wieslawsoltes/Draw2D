// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders
{
    [DataContract(IsReference = true)]
    public class SweepGradientShader : ViewModelBase, IShader
    {
        // TODO:

        public SweepGradientShader()
        {
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new SweepGradientShader()
            {
                Title = this.Title
            };
        }
    }
}
