using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Core2D.Model;

namespace Core2D.ViewModels
{
    public class DocumentViewModel : NodeViewModelBase<ProjectViewModel>
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public XDocument Document { get; }

        private ImmutableArray<PageViewModel> _pages;

        public ImmutableArray<PageViewModel> Pages
        {
            get { return _pages; }
            set { Update(ref _pages, value); }
        }

        public ObservableCommand NewPageCommand { get; }

        public ICommand NewDocumentBeforeCommand { get; }

        public ICommand NewDocumentAfterCommand { get; }

        public ICommand RemoveDocumentCommand { get; }

        public ICommand CutDocumentCommand { get; }

        public ICommand CopyDocumentCommand { get; }

        public ICommand PasteDocumentCommand { get; }

        public DocumentViewModel(XDocument document, ProjectViewModel owner)
        {
            Owner = owner;

            Document = document;

            _pages = ImmutableArray.CreateRange(Document.Pages.Select(i => new PageViewModel(i, this)));

            NewPageCommand = new ObservableCommand()
                .OnExecuted(x => NewPage(), Disposable)
                .AddTo(Disposable);

            NewDocumentBeforeCommand = new Command((p) => Owner.NewDocumentBefore(this), (p) => p != null);

            NewDocumentAfterCommand = new Command((p) => Owner.NewDocumentAfter(this), (p) => p != null);

            RemoveDocumentCommand = new Command((p) => Owner.RemoveDocument(this), (p) => p != null);

            CutDocumentCommand = new Command((p) => Owner.CutDocument(this), (p) => p != null);

            CopyDocumentCommand = new Command((p) => Owner.CopyDocument(this), (p) => p != null);

            PasteDocumentCommand = new Command((p) => Owner.PasteDocument(this), (p) => p != null && Clipboard.HasData && (Clipboard.DataType == typeof(XDocument) || Clipboard.DataType == typeof(XPage)));
        }

        public override void Dispose()
        {
            Disposable?.Dispose();
        }

        public void NewPage()
        {
            var page = ProjectFactory.CreatePage(Document, "Page");
            var vm = new PageViewModel(page, this);

            AddPage(vm);
        }

        public void NewPageBefore(PageViewModel before)
        {
            if (before != null)
            {
                var index = Document.Pages.IndexOf(before.Page);
                var page = ProjectFactory.CreatePage(Document, "Page");
                var vm = new PageViewModel(page, this);

                InsertPage(index, vm);
            }
        }

        public void NewPageAfter(PageViewModel after)
        {
            if (after != null)
            {
                var index = Document.Pages.IndexOf(after.Page) + 1;
                var page = ProjectFactory.CreatePage(Document, "Page");
                var vm = new PageViewModel(page, this);

                InsertPage(index, vm);
            }
        }

        public void AddPage(PageViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Document.Pages,
                    ViewModel = Pages
                },
                // Next
                new
                {
                    Model = Document.Pages.Add(vm.Page),
                    ViewModel = Pages.Add(vm)
                },
                // Transfer
                (state) =>
                {
                    Document.Pages = state.Model;
                    Pages = state.ViewModel;
                },
                "Add Page");

            snapshot.ToNext();
        }

        public void InsertPage(int index, PageViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Document.Pages,
                    ViewModel = Pages
                },
                // Next
                new
                {
                    Model = Document.Pages.Insert(index, vm.Page),
                    ViewModel = Pages.Insert(index, vm)
                },
                // Transfer
                (state) =>
                {
                    Document.Pages = state.Model;
                    Pages = state.ViewModel;
                },
                "Insert Page");

            snapshot.ToNext();
        }

        public void ReplacePage(int index, PageViewModel vm)
        {
            var builderModel = Document.Pages.ToBuilder();
            builderModel[index] = vm.Page;
            var nextModel = builderModel.ToImmutable();

            var builderViewModel = Pages.ToBuilder();
            builderViewModel[index] = vm;
            var nextViewModel = builderViewModel.ToImmutable();

            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Document.Pages,
                    ViewModel = Pages
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
                    Document.Pages = state.Model;
                    Pages = state.ViewModel;
                },
                "Replace Page");

            snapshot.ToNext();
        }

        public void RemovePage(PageViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Document.Pages,
                    ViewModel = Pages
                },
                // Next
                new
                {
                    Model = Document.Pages.Remove(vm.Page),
                    ViewModel = Pages.Remove(vm)
                },
                // Transfer
                (state) =>
                {
                    Document.Pages = state.Model;
                    Pages = state.ViewModel;
                },
                "Remove Page");

            snapshot.ToNext();
        }

        public void CutPage(PageViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Page;
            RemovePage(vm);
        }

        public void CopyPage(PageViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Page;
        }

        public void PastePage(PageViewModel vm)
        {
            var result = Clipboard.Data;
            if (result != null && result is XPage)
            {
                var page = Serializer.Clone<XPage>(result);
                ReplacePage(Pages.IndexOf(vm), new PageViewModel(page, this));
            }
            else if (result != null && result is XLayer)
            {
                var layer = Serializer.Clone<XLayer>(result);
                vm.AddLayer(new LayerViewModel(layer, vm));
            }
        }
    }
}
