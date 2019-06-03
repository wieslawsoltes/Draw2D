// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.Input;

namespace Draw2D.ViewModels.Tools
{
    [DataContract(IsReference = true)]
    public class MoveTool : BaseTool, ITool
    {
        private MoveToolSettings _settings;
        private PathTool _pathTool;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public PathTool PathTool
        {
            get => _pathTool;
            set => Update(ref _pathTool, value);
        }

        [IgnoreDataMember]
        public string Title => "Move";

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public MoveToolSettings Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        public MoveTool(PathTool pathTool)
        {
            PathTool = pathTool;
        }

        public void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            PathTool.Move(context);
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
