using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Core2D.Clipboard;
using Core2D.Factories;
using Core2D.FileSystem;
using Core2D.History;
using Core2D.Serializer;

namespace Core2D.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public override IVirtualClipboard Clipboard { get; }

        public override IJsonSerializer Serializer { get; }

        public override IHistory History { get; }

        public override IProjectFactory ProjectFactory { get; }

        public IFile ProjectFile { get; }

        private ProjectViewModel _project;

        public ProjectViewModel Project
        {
            get { return _project; }
            set { Update(ref _project, value); }
        }

        public ObservableCommand NewProjectCommand { get; }

        public ObservableCommand OpenProjectCommand { get; }

        public ObservableCommand CloseProjectCommand { get; }

        public ObservableCommand SaveProjectCommand { get; }

        public ObservableCommand UndoHistoryCommand { get; }

        public ObservableCommand RedoHistoryCommand { get; }

        public ObservableCommand ClearHistoryCommand { get; }

        public EditorViewModel(IVirtualClipboard clipboard, IJsonSerializer jsonSerializer, IHistory history, IProjectFactory projectFactory, IFile projectFile)
        {
            Clipboard = clipboard;

            Serializer = jsonSerializer;

            History = history;

            ProjectFactory = projectFactory;

            ProjectFile = projectFile;

            NewProjectCommand = new ObservableCommand()
                .OnExecuted(x => NewProject(), Disposable)
                .AddTo(Disposable);

            OpenProjectCommand = new ObservableCommand()
                .OnExecuted(p => OpenProject(), Disposable)
                .AddTo(Disposable);

            CloseProjectCommand = new ObservableCommand()
                .AddPredicate(this.ObserveProperty(x => x.Project, true).Select(x => x != null))
                .OnExecuted(p => CloseProject(), Disposable)
                .AddTo(Disposable);

            SaveProjectCommand = new ObservableCommand()
                .AddPredicate(this.ObserveProperty(x => x.Project, true).Select(x => x != null))
                .OnExecuted(p => SaveProject(), Disposable)
                .AddTo(Disposable);

            UndoHistoryCommand = new ObservableCommand()
                .AddPredicate(this.ObserveProperty(x => x.Project, true).Select(x => x != null))
                .AddPredicate(History.ObserveProperty(x => x.CanUndo, true))
                .OnExecuted(p => UndoHistory(), Disposable)
                .AddTo(Disposable);

            RedoHistoryCommand = new ObservableCommand()
                .AddPredicate(this.ObserveProperty(x => x.Project, true).Select(x => x != null))
                .AddPredicate(History.ObserveProperty(x => x.CanRedo, true))
                .OnExecuted(p => RedoHistory(), Disposable)
                .AddTo(Disposable);

            ClearHistoryCommand = new ObservableCommand()
                .AddPredicate(this.ObserveProperty(x => x.Project, true).Select(x => x != null))
                .AddPredicate(History.ObserveProperty(x => x.CanClear, true))
                .OnExecuted(p => ClearHistory(), Disposable)
                .AddTo(Disposable);
        }

        public override void Dispose()
        {
            Project?.Dispose();
            Disposable?.Dispose();
        }

        public void NewProject()
        {
            var project = ProjectFactory.CreateProject("project");
            Project?.Dispose();
            History.Clear();
            Project = new ProjectViewModel(project, this);
        }

        public void OpenProject()
        {
            var path = ProjectFile.GetOpenFileName();
            if (path != null)
            {
                OpenProject(path);
            }
        }

        public void OpenProject(string path)
        {
            if (path != null)
            {
                var project = Serializer.Open<Model.XProject>(path);
                Project?.Dispose();
                History.Clear();
                Project = new ProjectViewModel(project, this);
            }
        }

        public void CloseProject()
        {
            History.Clear();
            Project?.Dispose();
            Project = null;
        }

        public void SaveProject()
        {
            var path = ProjectFile.GetSaveFileName(Project.Project.Name);
            if (path != null)
            {
                SaveProject(path);
            }
        }

        public void SaveProject(string path)
        {
            Serializer.Save(path, Project.Project);
        }

        public void UndoHistory()
        {
            History?.Undo();
        }

        public void RedoHistory()
        {
            History?.Redo();
        }

        public void ClearHistory()
        {
            History?.Clear();
        }
    }
}
