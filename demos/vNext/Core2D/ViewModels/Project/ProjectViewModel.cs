using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows.Input;
using Core2D.Model;

namespace Core2D.ViewModels
{
    public class ProjectViewModel : NodeViewModelBase<EditorViewModel>
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public XProject Project { get; }

        private ImmutableArray<DocumentViewModel> _documents;

        public ImmutableArray<DocumentViewModel> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }

        public ICommand NewDocumentCommand { get; }

        public ProjectViewModel(XProject project, EditorViewModel owner)
        {
            Owner = owner;

            Project = project;

            _documents = ImmutableArray.CreateRange(Project.Documents.Select(i => new DocumentViewModel(i, this)));

            NewDocumentCommand = new Command((p) => NewDocument(), (p) => p == null);
        }

        public override void Dispose()
        {
            Disposable?.Dispose();
        }

        public void NewDocument()
        {
            var document = ProjectFactory.CreateDocument(Project, "Document");
            var vm = new DocumentViewModel(document, this);

            AddDocument(vm);
        }

        public void NewDocumentBefore(DocumentViewModel before)
        {
            if (before != null)
            {
                var index = Project.Documents.IndexOf(before.Document);
                var document = ProjectFactory.CreateDocument(Project, "Document");
                var vm = new DocumentViewModel(document, this);

                InsertDocument(index, vm);
            }
        }

        public void NewDocumentAfter(DocumentViewModel after)
        {
            if (after != null)
            {
                var index = Project.Documents.IndexOf(after.Document) + 1;
                var document = ProjectFactory.CreateDocument(Project, "Document");
                var vm = new DocumentViewModel(document, this);

                InsertDocument(index, vm);
            }
        }

        public void AddDocument(DocumentViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Project.Documents,
                    ViewModel = Documents
                },
                // Next
                new
                {
                    Model = Project.Documents.Add(vm.Document),
                    ViewModel = Documents.Add(vm)
                },
                // Transfer
                (state) =>
                {
                    Project.Documents = state.Model;
                    Documents = state.ViewModel;
                },
                "Add Document");

            snapshot.ToNext();
        }

        public void InsertDocument(int index, DocumentViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Project.Documents,
                    ViewModel = Documents
                },
                // Next
                new
                {
                    Model = Project.Documents.Insert(index, vm.Document),
                    ViewModel = Documents.Insert(index, vm)
                },
                // Transfer
                (state) =>
                {
                    Project.Documents = state.Model;
                    Documents = state.ViewModel;
                },
                "Insert Document");

            snapshot.ToNext();
        }

        public void ReplaceDocument(int index, DocumentViewModel vm)
        {
            var builderModel = Project.Documents.ToBuilder();
            builderModel[index] = vm.Document;
            var nextModel = builderModel.ToImmutable();

            var builderViewModel = Documents.ToBuilder();
            builderViewModel[index] = vm;
            var nextViewModel = builderViewModel.ToImmutable();

            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Project.Documents,
                    ViewModel = Documents
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
                    Project.Documents = state.Model;
                    Documents = state.ViewModel;
                },
                "Replace Document");

            snapshot.ToNext();
        }

        public void RemoveDocument(DocumentViewModel vm)
        {
            var snapshot = History.Snapshot(
                // Previous
                new
                {
                    Model = Project.Documents,
                    ViewModel = Documents
                },
                // Next
                new
                {
                    Model = Project.Documents.Remove(vm.Document),
                    ViewModel = Documents.Remove(vm)
                },
                // Transfer
                (state) =>
                {
                    Project.Documents = state.Model;
                    Documents = state.ViewModel;
                },
                "Remove Document");

            snapshot.ToNext();
        }

        public void CutDocument(DocumentViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Document;
            RemoveDocument(vm);
        }

        public void CopyDocument(DocumentViewModel vm)
        {
            Clipboard.Data = null;
            Clipboard.Data = vm.Document;
        }

        public void PasteDocument(DocumentViewModel vm)
        {
            var result = Clipboard.Data;
            if (result != null && result is XDocument)
            {
                var document = Serializer.Clone<XDocument>(result);
                ReplaceDocument(Documents.IndexOf(vm), new DocumentViewModel(document, this));
            }
            else if (result != null && result is XPage)
            {
                var page = Serializer.Clone<XPage>(result);
                vm.AddPage(new PageViewModel(page, vm));
            }
        }
    }
}
