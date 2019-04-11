// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Style;

namespace Draw2D.Editor
{
    public interface ICanvasEditor : IToolContext
    {
        T Load<T>(string path);
        void Save<T>(string path, T value);
        void New();
        void Open();
        void SaveAs();
        void Exit();
    }

    public class CanvasEditor : ToolContext, ICanvasEditor
    {
        public T Load<T>(string path)
        {
            var json = File.ReadAllText(path);
            return NewtonsoftJsonSerializer.FromJson<T>(json);
        }

        public void Save<T>(string path, T value)
        {
            var json = NewtonsoftJsonSerializer.ToJson<T>(value);
            File.WriteAllText(path, json);
        }

        public void New()
        {
            CurrentTool.Clean(this);
            Selection.Selected.Clear();
            var container = new CanvasContainer()
            {
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 128, 128, 128),
                InputBackground = new ArgbColor(255, 211, 211, 211)
            };
            var workingContainer = new CanvasContainer();
            CurrentContainer = container;
            WorkingContainer = new CanvasContainer();
            Invalidate?.Invoke();
        }

        public async void Open()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result.FirstOrDefault();
                var container = Load<CanvasContainer>(path);
                var workingContainer = new CanvasContainer();
                CurrentTool.Clean(this);
                Selection.Selected.Clear();
                CurrentContainer = container;
                WorkingContainer = workingContainer;
                Invalidate?.Invoke();
            }
        }

        public async void SaveAs()
        {
            var dlg = new SaveFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            dlg.InitialFileName = "container";
            dlg.DefaultExtension = "project";
            var result = await dlg.ShowAsync(Application.Current.Windows[0]);
            if (result != null)
            {
                var path = result;
                Save(path, CurrentContainer);
            }
        }

        public void Exit()
        {
            Application.Current.Windows.FirstOrDefault()?.Close();
        }
    }
}
