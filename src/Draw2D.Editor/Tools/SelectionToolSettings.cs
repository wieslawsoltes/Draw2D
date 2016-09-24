using System;
using Draw2D.Editor.Selection;
using Draw2D.Models.Style;

namespace Draw2D.Editor.Tools
{
    public class SelectionToolSettings : SettingsBase
    {
        private SelectionMode _mode;
        private SelectionTargets _targets;
        private DrawStyle _selectionStyle;
        private double _hitTestRadius;
        private bool _connectPoints;
        private bool _disconnectPoints;
        private double _connectTestRadius;
        private double _disconnectTestRadius;

        public SelectionMode Mode
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

        public SelectionTargets Targets
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

        public bool ConnectPoints
        {
            get { return _connectPoints; }
            set
            {
                if (value != _connectPoints)
                {
                    _connectPoints = value;
                    Notify("ConnectPoints");
                }
            }
        }

        public double ConnectTestRadius
        {
            get { return _connectTestRadius; }
            set
            {
                if (value != _connectTestRadius)
                {
                    _connectTestRadius = value;
                    Notify("ConnectTestRadius");
                }
            }
        }

        public bool DisconnectPoints
        {
            get { return _disconnectPoints; }
            set
            {
                if (value != _disconnectPoints)
                {
                    _disconnectPoints = value;
                    Notify("DisconnectPoints");
                }
            }
        }

        public double DisconnectTestRadius
        {
            get { return _disconnectTestRadius; }
            set
            {
                if (value != _disconnectTestRadius)
                {
                    _disconnectTestRadius = value;
                    Notify("DisconnectTestRadius");
                }
            }
        }
    }
}
