// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Style;

namespace Draw2D.Core.Editor.Tools
{
    public class GuideToolSettings : SettingsBase
    {
        private DrawStyle _guideStyle;
        
        public DrawStyle GuideStyle
        {
            get => _guideStyle;
            set => Update(ref _guideStyle, value);
        }
    }
}
