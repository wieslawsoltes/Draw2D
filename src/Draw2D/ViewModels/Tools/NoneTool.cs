// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels.Tools
{
    public class NoneToolSettings : SettingsBase
    {
    }

    public class NoneTool : ToolBase
    {
        public override string Title => "None";

        public NoneToolSettings Settings { get; set; }
    }
}
