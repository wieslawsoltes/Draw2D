using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Core2D.Model;

namespace Core2D.ViewModels
{
    public sealed class ShapeViewModel : NodeViewModelBase<LayerViewModel>
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public XShape Shape { get; }

        private ImmutableArray<PropertyViewModel> _properties;

        public ImmutableArray<PropertyViewModel> Properties
        {
            get { return _properties; }
            set { Update(ref _properties, value); }
        }

        public ICommand NewPropertyCommand { get; }

        public ICommand NewShapeBeforeCommand { get; }

        public ICommand NewShapeAfterCommand { get; }

        public ICommand RemoveShapeCommand { get; }

        public ICommand RemoveSelfCommand { get; }

        public ICommand CutShapeCommand { get; }

        public ICommand CopyShapeCommand { get; }

        public ICommand PasteShapeCommand { get; }

        public ShapeViewModel(XShape shape, LayerViewModel owner)
        {
            Owner = owner;

            Shape = shape;

            _properties = ImmutableArray.CreateRange(Shape.Properties.Select(i => new PropertyViewModel(i, this)));

            NewPropertyCommand = new ObservableCommand()
                .OnExecuted(x => NewProperty(), Disposable)
                .AddTo(Disposable);

            NewShapeBeforeCommand = new Command((p) => Owner.NewShapeBefore(this));

            NewShapeAfterCommand = new Command((p) => Owner.NewShapeAfter(this));

            RemoveShapeCommand = new Command((p) => Owner.RemoveShape(this));

            CutShapeCommand = new Command((p) => Owner.CutShape(this));

            CopyShapeCommand = new Command((p) => Owner.CopyShape(this));

            PasteShapeCommand = new Command((p) => Owner.PasteShape(this), (p) =>Clipboard.HasData && (Clipboard.DataType == typeof(XShape) || Clipboard.DataType == typeof(XProperty)));
        }

        public override void Dispose()
        {
            Disposable?.Dispose();
        }

        public void NewProperty()
        {
            var property = ProjectFactory.CreateProperty(Shape, "Property", "");
            var vm = new PropertyViewModel(property, this);

            AddProperty(vm);
        }

        public void NewPropertyBefore(PropertyViewModel before)
        {
            if (before != null)
            {
                var index = Shape.Properties.IndexOf(before.Property);
                var property = ProjectFactory.CreateProperty(Shape, "Property", "");
                var vm = new PropertyViewModel(property, this);

                InsertProperty(index, vm);
            }
        }

        public void NewPropertyAfter(PropertyViewModel after)
        {
            if (after != null)
            {
                var index = Shape.Properties.IndexOf(after.Property) + 1;
                var property = ProjectFactory.CreateProperty(Shape, "Property", "");
                var vm = new PropertyViewModel(property, this);

                InsertProperty(index, vm);
            }
        }

        public void AddProperty(PropertyViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Shape.Properties,
                    ViewModel = Properties
                },
                // Next
                new
                {
                    Model = Shape.Properties.Add(vm.Property),
                    ViewModel = Properties.Add(vm)
                },
                // Transfer
                (state) =>
                {
                    Shape.Properties = state.Model;
                    Properties = state.ViewModel;
                },
                "Add Property");

            snapshot.ToNext();
        }

        public void InsertProperty(int index, PropertyViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Shape.Properties,
                    ViewModel = Properties
                },
                // Next
                new
                {
                    Model = Shape.Properties.Insert(index, vm.Property),
                    ViewModel = Properties.Insert(index, vm)
                },
                // Transfer
                (state) =>
                {
                    Shape.Properties = state.Model;
                    Properties = state.ViewModel;
                },
                "Insert Property");

            snapshot.ToNext();
        }

        public void ReplaceProperty(int index, PropertyViewModel vm)
        {
            var builderModel = Shape.Properties.ToBuilder();
            builderModel[index] = vm.Property;
            var nextModel = builderModel.ToImmutable();

            var builderViewModel = Properties.ToBuilder();
            builderViewModel[index] = vm;
            var nextViewModel = builderViewModel.ToImmutable();

            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Shape.Properties,
                    ViewModel = Properties
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
                    Shape.Properties = state.Model;
                    Properties = state.ViewModel;
                },
                "Replace Property");

            snapshot.ToNext();
        }

        public void RemoveProperty(PropertyViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Shape.Properties,
                    ViewModel = Properties
                },
                // Next
                new
                {
                    Model = Shape.Properties.Remove(vm.Property),
                    ViewModel = Properties.Remove(vm)
                },
                // Transfer
                (state) =>
                {
                    Shape.Properties = state.Model;
                    Properties = state.ViewModel;
                },
                "Remove Property");

            snapshot.ToNext();
        }

        public void CutProperty(PropertyViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Property;
            RemoveProperty(vm);
        }

        public void CopyProperty(PropertyViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Property;
        }

        public void PasteProperty(PropertyViewModel vm)
        {
            var result = Clipboard.Data;
            if (result != null && result is XProperty)
            {
                var property = Serializer.Clone<XProperty>(result);
                ReplaceProperty(Properties.IndexOf(vm), new PropertyViewModel(property, this));
            }
        }
    }
}
