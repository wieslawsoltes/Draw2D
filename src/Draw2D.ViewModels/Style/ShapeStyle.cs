// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style
{
    [DataContract(IsReference = true)]
    public class ShapeStyle : ViewModelBase, IShapeStyle
    {
        private bool _isStroked;
        private bool _isFilled;
        private bool _isText;
        private IPaint _strokePaint;
        private IPaint _fillPaint;
        private IPaint _textPaint;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsText
        {
            get => _isText;
            set => Update(ref _isText, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaint StrokePaint
        {
            get => _strokePaint;
            set => Update(ref _strokePaint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaint FillPaint
        {
            get => _fillPaint;
            set => Update(ref _fillPaint, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IPaint TextPaint
        {
            get => _textPaint;
            set => Update(ref _textPaint, value);
        }

        public ShapeStyle()
        {
        }

        public ShapeStyle(
            string title,
            IPaint strokePaint = null,
            IPaint fillPaint = null,
            IPaint textPaint = null,
            bool isStroked = true,
            bool isFilled = false,
            bool isText = true)
        {
            this.Title = title;
            this.IsStroked = isStroked;
            this.IsFilled = isFilled;
            this.IsText = isText;
            this.StrokePaint = strokePaint;
            this.FillPaint = fillPaint;
            this.TextPaint = textPaint;
        }

        public override void Invalidate()
        {
            _strokePaint?.Invalidate();
            _strokePaint?.Color?.Invalidate();
            _strokePaint?.Typeface?.Invalidate();
            _strokePaint?.Effects?.Invalidate();
            _strokePaint?.Effects?.ColorFilter?.Invalidate();
            _strokePaint?.Effects?.MaskFilter?.Invalidate();
            _strokePaint?.Effects?.PathEffect?.Invalidate();
            _strokePaint?.Effects?.Shader?.Invalidate();

            _fillPaint?.Invalidate();
            _fillPaint?.Color?.Invalidate();
            _fillPaint?.Typeface?.Invalidate();
            _fillPaint?.Effects?.ColorFilter?.Invalidate();
            _fillPaint?.Effects?.MaskFilter?.Invalidate();
            _fillPaint?.Effects?.PathEffect?.Invalidate();
            _fillPaint?.Effects?.Shader?.Invalidate();

            _textPaint?.Invalidate();
            _textPaint?.Color?.Invalidate();
            _textPaint?.Typeface?.Invalidate();
            _textPaint?.Effects?.ColorFilter?.Invalidate();
            _textPaint?.Effects?.MaskFilter?.Invalidate();
            _textPaint?.Effects?.PathEffect?.Invalidate();
            _textPaint?.Effects?.Shader?.Invalidate();

            base.Invalidate();
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Title = this.Title + "_copy",
                IsStroked = this.IsStroked,
                IsFilled = this.IsFilled,
                IsText = this.IsText,
                StrokePaint = (IPaint)this.StrokePaint.Copy(shared),
                FillPaint = (IPaint)this.FillPaint.Copy(shared),
                TextPaint = (IPaint)this.TextPaint.Copy(shared)
            };
        }
    }
}
