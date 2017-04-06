// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.Core.Editor
{
    public interface IEdit
    {
        void Cut(IToolContext context);
        void Copy(IToolContext context);
        void Paste(IToolContext context);
        void Delete(IToolContext context);
        void Group(IToolContext context);
    }
}
