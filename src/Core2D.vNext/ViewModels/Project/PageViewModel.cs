using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Core2D.Model;

namespace Core2D.ViewModels
{
    public class PageViewModel : NodeViewModelBase<DocumentViewModel>
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public XPage Page { get; }

        private ImmutableArray<LayerViewModel> _layers;

        public ImmutableArray<LayerViewModel> Layers
        {
            get { return _layers; }
            set { Update(ref _layers, value); }
        }

        public ICommand NewLayerCommand { get; }

        public ObservableCommand NewPageBeforeCommand { get; }

        public ObservableCommand NewPageAfterCommand { get; }

        public ObservableCommand RemovePageCommand { get; }

        public ObservableCommand CutPageCommand { get; }

        public ObservableCommand CopyPageCommand { get; }

        public ObservableCommand PastePageCommand { get; }

        public PageViewModel(XPage page, DocumentViewModel owner)
        {
            Owner = owner;

            Page = page;

            _layers = ImmutableArray.CreateRange(Page.Layers.Select(i => new LayerViewModel(i, this)));

            NewLayerCommand = new Command((p) => NewLayer(), (p) => p == null);

            NewPageBeforeCommand = new ObservableCommand()
                .OnExecuted(x => Owner.NewPageBefore(this), Disposable)
                .AddTo(Disposable);

            NewPageAfterCommand = new ObservableCommand()
                .OnExecuted(x => Owner.NewPageAfter(this), Disposable)
                .AddTo(Disposable);

            RemovePageCommand = new ObservableCommand()
                .OnExecuted(x => Owner.RemovePage(this), Disposable)
                .AddTo(Disposable);

            CutPageCommand = new ObservableCommand()
                .OnExecuted(x => Owner.CutPage(this), Disposable)
                .AddTo(Disposable);

            CopyPageCommand = new ObservableCommand()
                .OnExecuted(x => Owner.CopyPage(this), Disposable)
                .AddTo(Disposable);

            PastePageCommand = new ObservableCommand()
                .AddPredicate(Clipboard.ObserveProperty(x => x.HasData, true))
                .AddPredicate(Clipboard.ObserveProperty(x => x.DataType, true).Select(x => x == typeof(XPage) || x == typeof(XLayer)))
                .OnExecuted(x => Owner.PastePage(this), Disposable)
                .AddTo(Disposable);
        }

        public override void Dispose()
        {
            Disposable?.Dispose();
        }

        public void NewLayer()
        {
            var layer = ProjectFactory.CreateLayer(Page, "Layer");
            var vm = new LayerViewModel(layer, this);

            AddLayer(vm);
        }

        public void NewLayerBefore(LayerViewModel before)
        {
            if (before != null)
            {
                var index = Page.Layers.IndexOf(before.Layer);
                var layer = ProjectFactory.CreateLayer(Page, "Layer");
                var vm = new LayerViewModel(layer, this);

                InsertLayer(index, vm);
            }
        }

        public void NewLayerAfter(LayerViewModel after)
        {
            if (after != null)
            {
                var index = Page.Layers.IndexOf(after.Layer) + 1;
                var layer = ProjectFactory.CreateLayer(Page, "Layer");
                var vm = new LayerViewModel(layer, this);

                InsertLayer(index, vm);
            }
        }

        public void AddLayer(LayerViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Page.Layers,
                    ViewModel = Layers
                },
                // Next
                new
                {
                    Model = Page.Layers.Add(vm.Layer),
                    ViewModel = Layers.Add(vm)
                },
                // Transfer
                (state) =>
                {
                    Page.Layers = state.Model;
                    Layers = state.ViewModel;
                },
                "Add Layer");

            snapshot.ToNext();
        }

        public void InsertLayer(int index, LayerViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Page.Layers,
                    ViewModel = Layers
                },
                // Next
                new
                {
                    Model = Page.Layers.Insert(index, vm.Layer),
                    ViewModel = Layers.Insert(index, vm)
                },
                // Transfer
                (state) =>
                {
                    Page.Layers = state.Model;
                    Layers = state.ViewModel;
                },
                "Insert Layer");

            snapshot.ToNext();
        }

        public void ReplaceLayer(int index, LayerViewModel vm)
        {
            var builderModel = Page.Layers.ToBuilder();
            builderModel[index] = vm.Layer;
            var nextModel = builderModel.ToImmutable();

            var builderViewModel = Layers.ToBuilder();
            builderViewModel[index] = vm;
            var nextViewModel = builderViewModel.ToImmutable();

            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Page.Layers,
                    ViewModel = Layers
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
                    Page.Layers = state.Model;
                    Layers = state.ViewModel;
                },
                "Replace Layer");

            snapshot.ToNext();
        }

        public void RemoveLayer(LayerViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Page.Layers,
                    ViewModel = Layers
                },
                // Next
                new
                {
                    Model = Page.Layers.Remove(vm.Layer),
                    ViewModel = Layers.Remove(vm)
                },
                // Transfer
                (state) =>
                {
                    Page.Layers = state.Model;
                    Layers = state.ViewModel;
                },
                "Remove Layer");

            snapshot.ToNext();
        }

        public void CutLayer(LayerViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Layer;
            RemoveLayer(vm);
        }

        public void CopyLayer(LayerViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Layer;
        }

        public void PasteLayer(LayerViewModel vm)
        {
            var result = Clipboard.Data;
            if (result != null && result is XLayer)
            {
                var layer = Serializer.Clone<XLayer>(result);
                ReplaceLayer(Layers.IndexOf(vm), new LayerViewModel(layer, this));
            }
            else if (result != null && result is XShape)
            {
                var shape = Serializer.Clone<XShape>(result);
                vm.AddShape(new ShapeViewModel(shape, vm));
            }
        }
    }
}
