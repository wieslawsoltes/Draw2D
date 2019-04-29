// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels;

namespace Draw2D.Editor
{
    public interface ICanvasEditor : IToolContext
    {
        T Load<T>(string path);
        void Save<T>(string path, T value);
        void New();
        void Open();
        void SaveAs();
        void OpenContainer(string path);
        void SaveContainer(string path);
        void Exit();
    }
}
