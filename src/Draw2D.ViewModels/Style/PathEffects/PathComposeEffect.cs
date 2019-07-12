// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathComposeEffect : ViewModelBase, IPathEffect
    {
        public static IPathEffectFactory PathEffectFactory { get; } = new PathEffectFactory();

        private IPathEffect _outer;
        private IPathEffect _inner;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPathEffect Outer
        {
            get => _outer;
            set => Update(ref _outer, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPathEffect Inner
        {
            get => _inner;
            set => Update(ref _inner, value);
        }

        public PathComposeEffect()
        {
        }

        public PathComposeEffect(IPathEffect outer, IPathEffect inner)
        {
            this.Outer = outer;
            this.Inner = inner;
        }

        public static IPathEffect MakeCompose()
        {
            return new PathComposeEffect();
        }

        public void SetOuterPathEffect(IPathEffect pathEffect)
        {
            this.Outer = pathEffect;
        }

        public void SetInnerPathEffect(IPathEffect pathEffect)
        {
            this.Inner = pathEffect;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathComposeEffect()
            {
                Title = this.Title,
                Outer = (IPathEffect)this.Outer.Copy(shared),
                Inner = (IPathEffect)this.Inner.Copy(shared)
            };
        }
    }
}
