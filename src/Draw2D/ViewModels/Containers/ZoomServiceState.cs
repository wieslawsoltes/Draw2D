// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;
using Draw2D.Input;

namespace Draw2D.ViewModels.Containers
{
    [DataContract(IsReference = true)]
    public class ZoomServiceState : ViewModelBase, IZoomServiceState
    {
        private double _zoomSpeed;
        private double _zoomX;
        private double _zoomY;
        private double _offsetX;
        private double _offsetY;
        private bool _isPanning;
        private bool _isZooming;
        private FitMode _initFitMode;
        private FitMode _autoFitMode;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomSpeed
        {
            get => _zoomSpeed;
            set => Update(ref _zoomSpeed, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomX
        {
            get => _zoomX;
            set => Update(ref _zoomX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double ZoomY
        {
            get => _zoomY;
            set => Update(ref _zoomY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsPanning
        {
            get => _isPanning;
            set => Update(ref _isPanning, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsZooming
        {
            get => _isZooming;
            set => Update(ref _isZooming, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FitMode InitFitMode
        {
            get => _initFitMode;
            set => Update(ref _initFitMode, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public FitMode AutoFitMode
        {
            get => _autoFitMode;
            set => Update(ref _autoFitMode, value);
        }
    }
}
