// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Renderer;
using Draw2D.ViewModels.Style;

namespace Draw2D.ViewModels
{
    /// <summary>
    /// Defines drawable shape contract.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Get or sets shape drawing style.
        /// </summary>
        ShapeStyle Style { get; set; }

        /// <summary>
        /// Get or sets shape matrix transform.
        /// </summary>
        MatrixObject Transform { get; set; }

        /// <summary>
        /// Begins matrix transform.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <returns>The previous transform state.</returns>
        object BeginTransform(object dc, IShapeRenderer renderer);

        /// <summary>
        /// Ends matrix transform.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="state">The previous transform state.</param>
        void EndTransform(object dc, IShapeRenderer renderer, object state);

        /// <summary>
        /// Draws shape using current <see cref="IShapeRenderer"/>.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        /// <param name="db">The properties database.</param>
        /// <param name="r">The database record.</param>
        void Draw(object dc, IShapeRenderer renderer, double dx, double dy, object db, object r);

        /// <summary>
        /// Invalidates shape renderer cache.
        /// </summary>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        /// <returns>Returns true if shape was invalidated; otherwise, returns false.</returns>
        bool Invalidate(IShapeRenderer renderer, double dx, double dy);
    }
}
