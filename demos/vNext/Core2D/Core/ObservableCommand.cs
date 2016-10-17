using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Core2D
{
    public class ObservableCommand : ICommand, IDisposable
    {
        private Subject<object> ExecutedTrigger { get; } = new Subject<object>();
        private Subject<Exception> ExeceptionsTrigger { get; } = new Subject<Exception>();
        private List<IObservable<bool>> Predicates { get; } = new List<IObservable<bool>>();
        private IObservable<bool> CanExecute { get; set; }
        private bool Latest { get; set; } = true;
        private CompositeDisposable Disposable { get; set; } = new CompositeDisposable();

        public ObservableCommand()
            : this(true)
        {
        }

        public ObservableCommand(bool initial)
        {
            RaiseCanExecute(initial);
        }

        public ObservableCommand(IObservable<bool> predicate)
            : this(predicate, true)
        {
        }

        public ObservableCommand(IObservable<bool> predicate, bool initial)
        {
            if (predicate != null)
            {
                CanExecute = predicate.DistinctUntilChanged();
                Subscribe();
            }
            RaiseCanExecute(initial);
        }

        public ObservableCommand AddPredicate(IObservable<bool> predicate)
        {
            Disposable.Dispose();
            Predicates.Add(predicate);
            CanExecute = CanExecute == null ?
                predicate.DistinctUntilChanged() :
                CanExecute.CombineLatest(Predicates.Last(), (a, b) => a && b).DistinctUntilChanged();
            Subscribe();
            return this;
        }

        public ObservableCommand AddTo(ICollection<IDisposable> disposables)
        {
            disposables.Add(this);
            return this;
        }

        public ObservableCommand OnExecuted(Action<object> onNext, ICollection<IDisposable> disposables)
        {
            disposables.Add(ExecutedTrigger.Subscribe(onNext));
            return this;
        }

        public ObservableCommand OnExeception(Action<object> onNext, ICollection<IDisposable> disposables)
        {
            disposables.Add(ExeceptionsTrigger.Subscribe(onNext));
            return this;
        }

        public event EventHandler CanExecuteChanged;

        bool ICommand.CanExecute(object parameter) => Latest;

        public void Execute(object parameter) => ExecutedTrigger.OnNext(parameter);

        public IObservable<object> Executed => ExecutedTrigger.AsObservable();

        public IObservable<Exception> Execeptions => ExeceptionsTrigger.AsObservable();

        public void Dispose() => Disposable.Dispose();

        protected virtual void RaiseCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }

        private void RaiseCanExecute(bool value)
        {
            Latest = value;
            RaiseCanExecuteChanged(EventArgs.Empty);
        }

        private void Subscribe()
        {
            Disposable = new CompositeDisposable();
            Disposable.Add(
                CanExecute.Subscribe(
                    x => RaiseCanExecute(x),
                    ExeceptionsTrigger.OnNext));
        }
    }
}
