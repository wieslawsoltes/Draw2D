using System.Collections.Generic;

namespace Draw2D.Core.Renderers
{
    public abstract class ShapeHelper
    {
        public abstract void Draw(object dc, ShapeRenderer r, ShapeObject shape, ISet<ShapeObject> selected);
    }
}
