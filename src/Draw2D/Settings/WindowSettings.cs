// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Avalonia.Controls;
using Draw2D.ViewModels;

namespace Draw2D.Settings
{
    [DataContract(IsReference = true)]
    public class WindowSettings : SettingsBase
    {
        private double _width;
        private double _height;
        private double _x;
        private double _y;
        private WindowState _windowState;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public WindowState WindowState
        {
            get => _windowState;
            set => Update(ref _windowState, value);
        }
    }
}
