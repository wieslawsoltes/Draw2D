// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.Input;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class NoneTool : BaseTool, ITool
    {
        private NoneToolSettings _settings;

        [IgnoreDataMember]
        public string Title => "None";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public NoneToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void LeftUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void RightUp(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Move(IToolContext context, double x, double y, Modifier modifier)
        {
        }

        public void Clean(IToolContext context)
        {
        }
    }
}
