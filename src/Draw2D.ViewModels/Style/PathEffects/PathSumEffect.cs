// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.PathEffects
{
    [DataContract(IsReference = true)]
    public class PathSumEffect : ViewModelBase, IPathEffect
    {
        public static IPathEffectFactory PathEffectFactory { get; } = Style.PathEffectFactory.Instance;

        private IPathEffect _first;
        private IPathEffect _second;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPathEffect First
        {
            get => _first;
            set => Update(ref _first, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPathEffect Second
        {
            get => _second;
            set => Update(ref _second, value);
        }

        public PathSumEffect()
        {
        }

        public PathSumEffect(IPathEffect first, IPathEffect second)
        {
            this.First = first;
            this.Second = second;
        }

        public static IPathEffect MakeSum()
        {
            return new PathSumEffect() { Title = "Sum" };
        }

        public void SetFirstPathEffect(IPathEffect pathEffect)
        {
            this.First = pathEffect;
        }

        public void SetSecondPathEffect(IPathEffect pathEffect)
        {
            this.Second = pathEffect;
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new PathSumEffect()
            {
                Title = this.Title,
                First = (IPathEffect)this.First.Copy(shared),
                Second = (IPathEffect)this.Second.Copy(shared)
            };
        }
    }
}
