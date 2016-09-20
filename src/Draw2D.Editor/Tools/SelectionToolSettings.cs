using System;
using Draw2D.Models.Style;

namespace Draw2D.Editor.Tools
{
    public class SelectionToolSettings : SettingsBase
    {
        private SelectionToolMode _mode;
        private SelectionToolTargets _targets;
        private DrawStyle _selectionStyle;
        private double _hitTestRadius;

        public SelectionToolMode Mode
        {
            get { return _mode; }
            set
            {
                if (value != _mode)
                {
                    _mode = value;
                    Notify("Mode");
                }
            }
        }

        public SelectionToolTargets Targets
        {
            get { return _targets; }
            set
            {
                if (value != _targets)
                {
                    _targets = value;
                    Notify("Targets");
                }
            }
        }

        public DrawStyle SelectionStyle
        {
            get { return _selectionStyle; }
            set
            {
                if (value != _selectionStyle)
                {
                    _selectionStyle = value;
                    Notify("SelectionStyle");
                }
            }
        }

        public double HitTestRadius
        {
            get { return _hitTestRadius; }
            set
            {
                if (value != _hitTestRadius)
                {
                    _hitTestRadius = value;
                    Notify("HitTestRadius");
                }
            }
        }
    }
}
