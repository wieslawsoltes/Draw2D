using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Core2D.Model;

namespace Core2D.ViewModels
{
    public class LayerViewModel : NodeViewModelBase<PageViewModel>
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public XLayer Layer { get; }

        private ImmutableArray<ShapeViewModel> _shapes;

        public ImmutableArray<ShapeViewModel> Shapes
        {
            get { return _shapes; }
            set { Update(ref _shapes, value); }
        }

        public ICommand NewShapeCommand { get; }

        public ICommand NewLayerBeforeCommand { get; }

        public ICommand NewLayerAfterCommand { get; }

        public ICommand RemoveLayerCommand { get; }

        public ICommand RemoveSelfCommand { get; }

        public ICommand CutLayerCommand { get; }

        public ICommand CopyLayerCommand { get; }

        public ICommand PasteLayerCommand { get; }

        public LayerViewModel(XLayer layer, PageViewModel owner)
        {
            Owner = owner;

            Layer = layer;

            _shapes = ImmutableArray.CreateRange(Layer.Shapes.Select(i => new ShapeViewModel(i, this)));

            NewShapeCommand = new Command((p) => NewShape());

            NewLayerBeforeCommand = new Command((p) => Owner.NewLayerBefore(this));

            NewLayerAfterCommand = new Command((p) => Owner.NewLayerAfter(this));

            RemoveLayerCommand = new Command((p) => Owner.RemoveLayer(this));

            CutLayerCommand = new Command((p) => Owner.CutLayer(this));

            CopyLayerCommand = new Command((p) => Owner.CopyLayer(this));

            PasteLayerCommand = new Command((p) => Owner.PasteLayer(this), (p) => Clipboard.HasData && (Clipboard.DataType == typeof(XLayer) || Clipboard.DataType == typeof(XShape)));
        }

        public override void Dispose()
        {
            Disposable?.Dispose();
        }

        public void NewShape()
        {
            var shape = ProjectFactory.CreateShape(Layer, "Shape");
            var vm = new ShapeViewModel(shape, this);

            AddShape(vm);
        }

        public void NewShapeBefore(ShapeViewModel before)
        {
            if (before != null)
            {
                var index = Layer.Shapes.IndexOf(before.Shape);
                var shape = ProjectFactory.CreateShape(Layer, "Shape");
                var vm = new ShapeViewModel(shape, this);

                InsertShape(index, vm);
            }
        }

        public void NewShapeAfter(ShapeViewModel after)
        {
            if (after != null)
            {
                var index = Layer.Shapes.IndexOf(after.Shape) + 1;
                var shape = ProjectFactory.CreateShape(Layer, "Shape");
                var vm = new ShapeViewModel(shape, this);

                InsertShape(index, vm);
            }
        }

        public void AddShape(ShapeViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Layer.Shapes,
                    ViewModel = Shapes
                },
                // Next
                new
                {
                    Model = Layer.Shapes.Add(vm.Shape),
                    ViewModel = Shapes.Add(vm)
                },
                // Transfer
                (state) =>
                {
                    Layer.Shapes = state.Model;
                    Shapes = state.ViewModel;
                },
                "Add Shape");

            snapshot.ToNext();
        }

        public void InsertShape(int index, ShapeViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Layer.Shapes,
                    ViewModel = Shapes
                },
                // Next
                new
                {
                    Model = Layer.Shapes.Insert(index, vm.Shape),
                    ViewModel = Shapes.Insert(index, vm)
                },
                // Transfer
                (state) =>
                {
                    Layer.Shapes = state.Model;
                    Shapes = state.ViewModel;
                },
                "Insert Shape");

            snapshot.ToNext();
        }

        public void ReplaceShape(int index, ShapeViewModel vm)
        {
            var builderModel = Layer.Shapes.ToBuilder();
            builderModel[index] = vm.Shape;
            var nextModel = builderModel.ToImmutable();

            var builderViewModel = Shapes.ToBuilder();
            builderViewModel[index] = vm;
            var nextViewModel = builderViewModel.ToImmutable();

            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Layer.Shapes,
                    ViewModel = Shapes
                },
                // Next
                new
                {
                    Model = nextModel,
                    ViewModel = nextViewModel
                },
                // Transfer
                (state) =>
                {
                    Layer.Shapes = state.Model;
                    Shapes = state.ViewModel;
                },
                "Replace Shape");

            snapshot.ToNext();
        }

        public void RemoveShape(ShapeViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Layer.Shapes,
                    ViewModel = Shapes
                },
                // Next
                new
                {
                    Model = Layer.Shapes.Remove(vm.Shape),
                    ViewModel = Shapes.Remove(vm)
                },
                // Transfer
                (state) =>
                {
                    Layer.Shapes = state.Model;
                    Shapes = state.ViewModel;
                },
                "Remove Shape");

            snapshot.ToNext();
        }

        public void CutShape(ShapeViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Shape;
            RemoveShape(vm);
        }

        public void CopyShape(ShapeViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Shape;
        }

        public void PasteShape(ShapeViewModel vm)
        {
            var result = Clipboard.Data;
            if (result != null && result is XShape)
            {
                var shape = Serializer.Clone<XShape>(result);
                ReplaceShape(Shapes.IndexOf(vm), new ShapeViewModel(shape, this));
            }
            else if (result != null && result is XProperty)
            {
                var property = Serializer.Clone<XProperty>(result);
                vm.AddProperty(new PropertyViewModel(property, vm));
            }
        }
    }
}
