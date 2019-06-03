// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Draw2D.ViewModels
{
    public interface IConnectable
    {
        IList<IPointShape> Points { get; set; }
        bool Connect(IPointShape point, IPointShape target);
        bool Disconnect(IPointShape point, out IPointShape result);
        bool Disconnect();
    }
}
