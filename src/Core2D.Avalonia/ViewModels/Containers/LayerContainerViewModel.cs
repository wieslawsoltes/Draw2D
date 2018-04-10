// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Core2D.Containers;
using Core2D.Editor;
using Core2D.Editor.Bounds;
using Core2D.Editor.Tools;
using Core2D.Json;
using Core2D.Presenters;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;

namespace Core2D.ViewModels.Containers
{
    public class LayerContainerViewModel : ObservableObject, IToolContext
    {
        private ObservableCollection<ToolBase> _tools;
        private ToolBase _currentTool;
        private EditMode _mode;
        private ShapePresenter _presenter;
        private ShapeRenderer _renderer;
        private ISet<BaseShape> _selected;
        private IHitTest _hitTest;
        private ILayerContainer _currentContainer;
        private ILayerContainer _workingContainer;
        private ShapeStyle _currentStyle;
        private BaseShape _pointShape;

        public ObservableCollection<ToolBase> Tools
        {
            get => _tools;
            set => Update(ref _tools, value);
        }

        public ToolBase CurrentTool
        {
            get => _currentTool;
            set
            {
                _currentTool?.Clean(this);
                Update(ref _currentTool, value);
            }
        }

        public EditMode Mode
        {
            get => _mode;
            set => Update(ref _mode, value);
        }

        public ShapePresenter Presenter
        {
            get => _presenter;
            set => Update(ref _presenter, value);
        }

        public ShapeRenderer Renderer
        {
            get => _renderer;
            set => Update(ref _renderer, value);
        }

        public ISet<BaseShape> Selected
        {
            get => _selected;
            set => Update(ref _selected, value);
        }

        public IHitTest HitTest
        {
            get => _hitTest;
            set => Update(ref _hitTest, value);
        }

        public ILayerContainer CurrentContainer
        {
            get => _currentContainer;
            set => Update(ref _currentContainer, value);
        }

        public ILayerContainer WorkingContainer
        {
            get => _workingContainer;
            set => Update(ref _workingContainer, value);
        }

        public ShapeStyle CurrentStyle
        {
            get => _currentStyle;
            set => Update(ref _currentStyle, value);
        }

        public BaseShape PointShape
        {
            get => _pointShape;
            set => Update(ref _pointShape, value);
        }

        public Action Capture { get; set; }

        public Action Release { get; set; }

        public Action Invalidate { get; set; }

        public Action Reset { get; set; }

        public Action AutoFit { get; set; }

        public PointShape GetNextPoint(double x, double y, bool connect, double radius)
        {
            if (connect == true)
            {
                var point = HitTest.TryToGetPoint(CurrentContainer.Shapes, new Point2(x, y), radius, null);
                if (point != null)
                {
                    return point;
                }
            }
            return new PointShape(x, y, PointShape);
        }

        public void New()
        {
            CurrentTool.Clean(this);
            Renderer.Selected.Clear();
            var container = new LayerContainer()
            {
                Width = 720,
                Height = 630
            };
            var workingContainer = new LayerContainer();
            CurrentContainer = container;
            WorkingContainer = new LayerContainer();
            Invalidate?.Invoke();
        }

        public void OpenAsJson(string path)
        {
            var json = File.ReadAllText(path);
            var container = NewtonsoftJsonSerializer.FromJson<LayerContainer>(json);
            var workingContainer = new LayerContainer();
            CurrentTool.Clean(this);
            Renderer.Selected.Clear();
            CurrentContainer = container;
            WorkingContainer = workingContainer;
        }

        public void SaveAsJson(string path)
        {
            var json = NewtonsoftJsonSerializer.ToJson(CurrentContainer);
            File.WriteAllText(path, json);
        }

        public async void Open()
        {
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "Json Files", Extensions = { "json" } });
            dlg.Filters.Add(new FileDialogFilter() { Name = "All Files", Extensions = { "*" } });
            var result = await dlg.ShowAsync();
            if (result != null)
            {
                var path = result.FirstOrDefault();
                OpenAsJson(path);
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
            var result = await dlg.ShowAsync(Window.OpenWindows[0]);
            if (result != null)
            {
                var path = result;
                SaveAsJson(path);
            }
        }

        public void Exit()
        {
            Window.OpenWindows.FirstOrDefault()?.Close();
        }

        public void Cut()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Cut(this);
            }
        }

        public void Copy()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Copy(this);
            }
        }

        public void Paste()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Paste(this);
            }
        }

        public void Delete()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Delete(this);
            }
        }

        public void Group()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.Group(this);
            }
        }

        public void SelectAll()
        {
            if (CurrentTool is SelectionTool selectionTool)
            {
                selectionTool.SelectAll(this);
            }
        }

        public void SetNoneTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "None").FirstOrDefault();
        }

        public void SetSelectionTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "Selection").FirstOrDefault();
        }

        public void SetLineTool()
        {
            if (CurrentTool is PathTool pathTool)
            {
                pathTool.CleanCurrentTool(this);
                pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "Line").FirstOrDefault();
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == "Line").FirstOrDefault();
            }
        }

        public void SetPointTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "Point").FirstOrDefault();
        }

        public void SetCubicBezierTool()
        {
            if (CurrentTool is PathTool pathTool)
            {
                pathTool.CleanCurrentTool(this);
                pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "CubicBezier").FirstOrDefault();
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == "CubicBezier").FirstOrDefault();
            }
        }

        public void SetQuadraticBezierTool()
        {
            if (CurrentTool is PathTool pathTool)
            {
                pathTool.CleanCurrentTool(this);
                pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "QuadraticBezier").FirstOrDefault();
            }
            else
            {
                CurrentTool = Tools.Where(t => t.Title == "QuadraticBezier").FirstOrDefault();
            }
        }

        public void SetPathTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "Path").FirstOrDefault();
        }

        public void SetMoveTool()
        {
            if (CurrentTool is PathTool pathTool)
            {
                pathTool.CleanCurrentTool(this);
                pathTool.Settings.CurrentTool = pathTool.Settings.Tools.Where(t => t.Title == "Move").FirstOrDefault();
            }
        }

        public void SetRectangleTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "Rectangle").FirstOrDefault();
        }

        public void SetEllipseTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "Ellipse").FirstOrDefault();
        }

        public void SetTextTool()
        {
            CurrentTool = Tools.Where(t => t.Title == "Text").FirstOrDefault();
        }

        public void ResetZoom()
        {
            Reset?.Invoke();
        }

        public void AutoFitZoom()
        {
            AutoFit?.Invoke();
        }
    }
}
