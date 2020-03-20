using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Style.Shaders
{
    [DataContract(IsReference = true)]
    public class BitmapShader : ViewModelBase, IShader
    {
        public static ShaderTileMode[] ShaderTileModeValues { get; } = (ShaderTileMode[])Enum.GetValues(typeof(ShaderTileMode));

        private string _bitmap;
        private ShaderTileMode _tileModeX;
        private ShaderTileMode _tileModeY;
        private Matrix _localMatrix;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Bitmap
        {
            get => _bitmap;
            set => Update(ref _bitmap, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShaderTileMode TileModeX
        {
            get => _tileModeX;
            set => Update(ref _tileModeX, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ShaderTileMode TileModeY
        {
            get => _tileModeY;
            set => Update(ref _tileModeY, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Matrix LocalMatrix
        {
            get => _localMatrix;
            set => Update(ref _localMatrix, value);
        }

        public BitmapShader()
        {
        }

        public BitmapShader(string bitmap, ShaderTileMode tileModeX, ShaderTileMode tileModeY, Matrix localMatrix = null)
        {
            this.Bitmap = bitmap;
            this.TileModeX = tileModeX;
            this.TileModeY = tileModeY;
            this.LocalMatrix = localMatrix;
        }

        public static IShader MakeClamp()
        {
            return new BitmapShader("", ShaderTileMode.Clamp, ShaderTileMode.Clamp);
        }

        public static IShader MakeRepeat()
        {
            return new BitmapShader("", ShaderTileMode.Repeat, ShaderTileMode.Repeat);
        }

        public static IShader MakeMirror()
        {
            return new BitmapShader("", ShaderTileMode.Mirror, ShaderTileMode.Mirror);
        }

        public static IShader MakeLocalMatrix()
        {
            return new BitmapShader("", ShaderTileMode.Repeat, ShaderTileMode.Repeat, Matrix.MakeIdentity());
        }

        public object Copy(Dictionary<object, object> shared)
        {
            return new BitmapShader()
            {
                Title = this.Title,
                Bitmap = this.Bitmap,
                TileModeX = this.TileModeX,
                TileModeY = this.TileModeY,
                LocalMatrix = (Matrix)this.LocalMatrix.Copy(shared)
            };
        }
    }
}
