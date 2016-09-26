using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Draw2D.Models;
using Draw2D.Models.Containers;
using Draw2D.ViewModels.Containers;
using Microsoft.Win32;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Draw2D.Wpf.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                New(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Yaml Files (*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 0
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                var vm = this.DataContext as ShapesContainerViewModel;
                if (vm != null)
                {
                    Open(path, vm);
                    RendererView.InvalidateVisual();
                }
            }
        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Yaml Files (*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "container"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                var vm = this.DataContext as ShapesContainerViewModel;
                if (vm != null)
                {
                    Save(path, vm);
                }
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[0].Close();
        }

        private void EditCut_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                Cut(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void EditCopy_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                Copy(vm);
            }
        }

        private void EditPaste_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                Paste(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void EditDelete_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ShapesContainerViewModel;
            if (vm != null)
            {
                Delete(vm);
                RendererView.InvalidateVisual();
            }
        }

        private static string ToYaml(object graph)
        {
            using (var writer = new StringWriter())
            {
                var builder = new SerializerBuilder()
                    .EnsureRoundtrip()
                    .WithNamingConvention(new NullNamingConvention());
                var serializer = builder.Build();
                serializer.Serialize(writer, graph);
                return writer.ToString();
            }
        }

        private static T FromYaml<T>(string yaml)
        {
            using (var reader = new StringReader(yaml))
            {
                var builder = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .WithNamingConvention(new NullNamingConvention());
                var deserializer = builder.Build();
                return deserializer.Deserialize<T>(reader);
            }
        }

        private void New(ShapesContainerViewModel vm)
        {
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            var container = new ShapesContainer()
            {
                Width = 720,
                Height = 630
            };
            var workingContainer = new ShapesContainer();
            vm.Container = container;
            vm.WorkingContainer = new ShapesContainer();
        }

        private void Open(string path, ShapesContainerViewModel vm)
        {
            var yaml = File.ReadAllText(path);
            var container = FromYaml<ShapesContainer>(yaml);
            var workingContainer = new ShapesContainer();
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            vm.Container = container;
            vm.WorkingContainer = workingContainer;
        }

        private void Save(string path, ShapesContainerViewModel vm)
        {
            var yaml = ToYaml(vm.Container);
            File.WriteAllText(path, yaml);
        }

        private void Cut(ShapesContainerViewModel vm)
        {
            Copy(vm);
            Delete(vm);
        }

        private void Copy(ShapesContainerViewModel vm)
        {
            var selected = vm.Renderer.Selected;
            var shapes = new List<BaseShape>();
            foreach (var shape in selected)
            {
                if (vm.Container.Shapes.Contains(shape))
                {
                    shapes.Add(shape);
                }
            }
            var yaml = ToYaml(shapes);
            Clipboard.SetText(yaml);
        }

        private void Paste(ShapesContainerViewModel vm)
        {
            if (Clipboard.ContainsText())
            {
                var yaml = Clipboard.GetText();
                var shapes = FromYaml<List<BaseShape>>(yaml);
                vm.Renderer.Selected.Clear();
                foreach (var shape in shapes)
                {
                    vm.Container.Shapes.Add(shape);
                    shape.Select(vm.Renderer.Selected);
                }
            }
        }

        private void Delete(ShapesContainerViewModel vm)
        {
            var selected = vm.Renderer.Selected;
            foreach (var shape in selected)
            {
                if (vm.Container.Shapes.Contains(shape))
                {
                    vm.Container.Shapes.Remove(shape);
                }
            }
            vm.Renderer.Selected.Clear();
        }
    }
}
