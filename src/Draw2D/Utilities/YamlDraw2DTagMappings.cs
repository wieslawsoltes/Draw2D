// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Utilities
{
    public class YamlDraw2DTagMappings
    {
        public static IDictionary<Type, string> TagMappings = new Dictionary<Type, string>
        {
            // Renderer
            { typeof(MatrixObject), "tag:yaml.org,2002:matrix" },
            // Style
            { typeof(DrawColor), "tag:yaml.org,2002:color" },
            { typeof(DrawStyle), "tag:yaml.org,2002:style" },
            // Shapes
            { typeof(PointShape), "tag:yaml.org,2002:point" },
            { typeof(LineShape), "tag:yaml.org,2002:line" },
            { typeof(CubicBezierShape), "tag:yaml.org,2002:cubic" },
            { typeof(QuadraticBezierShape), "tag:yaml.org,2002:quad" },
            { typeof(PathShape), "tag:yaml.org,2002:path" },
            { typeof(FigureShape), "tag:yaml.org,2002:figure" },
            { typeof(ScribbleShape), "tag:yaml.org,2002:scribble" },
            { typeof(RectangleShape), "tag:yaml.org,2002:rect" },
            { typeof(EllipseShape), "tag:yaml.org,2002:ellipse" },
            { typeof(GroupShape), "tag:yaml.org,2002:group" },
            // Container
            { typeof(ShapesContainer), "tag:yaml.org,2002:container" }
        };
    }
}
