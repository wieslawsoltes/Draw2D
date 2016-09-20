using System;
using Draw2D.Models.Style;

namespace Draw2D.Editor.Tools
{
    public class GuideToolSettings : SettingsBase
    {
        private DrawStyle _guideStyle;
        
        public DrawStyle GuideStyle
        {
            get { return _guideStyle; }
            set
            {
                if (value != _guideStyle)
                {
                    _guideStyle = value;
                    Notify("GuideStyle");
                }
            }
        }
    }
}
