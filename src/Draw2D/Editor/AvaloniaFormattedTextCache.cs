// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Media;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.Editor
{
    public struct AvaloniaFormattedTextCache : IDisposable
    {
        public readonly FormattedText FormattedText;
        public readonly Point Origin;

        public AvaloniaFormattedTextCache(FormattedText formattedText, Point origin)
        {
            FormattedText = formattedText;
            Origin = origin;
        }

        public void Dispose()
        {
        }

        public static AvaloniaFormattedTextCache FromTextShape(TextShape text, Rect rect)
        {
            var constraint = new Size(rect.Width, rect.Height);

            var formattedText = new FormattedText()
            {
                Text = text.Text.Value,
                Constraint = constraint,
                TextAlignment = TextAlignment.Center,
                Wrapping = TextWrapping.NoWrap,
                Typeface = new Typeface("Arial", 11)
            };

            var size = formattedText.Bounds.Size;

            // Vertical Alignment: Top
            //var top = new Point(
            //    rect.X,
            //    rect.Y);

            // Vertical Alignment: Center
            var center = new Point(
                rect.X,
                rect.Y + rect.Height / 2 - size.Height / 2);

            // Vertical Alignment: Bottom
            //var bottom = new Point(
            //    rect.X,
            //    rect.Y + rect.Height - size.Height);

            return new AvaloniaFormattedTextCache(formattedText, center);
        }
    }
}
