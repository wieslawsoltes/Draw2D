// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Editor.Tools
{
    public class ScribbleToolSettings : SettingsBase
    {
        private bool _simplify;
        private double _epsilon;

        public bool Simplify
        {
            get => _simplify;
            set => Update(ref _simplify, value);
        }

        public double Epsilon
        {
            get => _epsilon;
            set => Update(ref _epsilon, value);
        }
    }
}
