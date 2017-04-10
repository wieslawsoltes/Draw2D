// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core.Shapes;

namespace Draw2D.Core
{
    public interface IConnectable
    {
        bool Connect(PointShape point, PointShape target);
        bool Disconnect(PointShape point, out PointShape result);
        bool Disconnect();
    }
}
