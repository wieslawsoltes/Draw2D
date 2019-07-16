// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public abstract class BasePaint : ViewModelBase, IPaint
    {
        private ArgbColor _color;
        private bool _isAntialias;
        private IPaintEffects _effects;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ArgbColor Color
        {
            get => _color;
            set => Update(ref _color, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsAntialias
        {
            get => _isAntialias;
            set => Update(ref _isAntialias, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaintEffects Effects
        {
            get => _effects;
            set => Update(ref _effects, value);
        }

        public abstract object Copy(Dictionary<object, object> shared);
    }
}
