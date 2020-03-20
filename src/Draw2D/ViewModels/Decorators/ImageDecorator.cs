using System.Runtime.Serialization;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    [DataContract(IsReference = true)]
    public class ImageDecorator : CommonDecorator
    {
        public void Draw(object dc, IShapeRenderer renderer, ImageShape imageShape, double dx, double dy, double scale)
        {
            DrawRectangle(dc, renderer, imageShape.StartPoint, imageShape.Point, dx, dy, scale);
        }

        public override void Draw(object dc, IBaseShape shape, IShapeRenderer renderer, ISelectionState selectionState, double dx, double dy, double scale)
        {
            if (shape is ImageShape imageShape)
            {
                Draw(dc, renderer, imageShape, dx, dy, scale);
            }
        }
    }
}
