using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Core2D.Model;

namespace Core2D.ViewModels
{
    public sealed class PropertyViewModel : NodeViewModelBase<ShapeViewModel>
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public XProperty Property { get; }

        public ICommand NewPropertyBeforeCommand { get; }

        public ICommand NewPropertyAfterCommand { get; }

        public ICommand RemovePropertyCommand { get; }

        public ICommand CutPropertyCommand { get; }

        public ICommand CopyPropertyCommand { get; }

        public ICommand PastePropertyCommand { get; }

        public PropertyViewModel(XProperty property, ShapeViewModel owner)
        {
            Owner = owner;

            Property = property;

            NewPropertyBeforeCommand = new ObservableCommand()
                .OnExecuted(x => Owner.NewPropertyBefore(this), Disposable)
                .AddTo(Disposable);

            NewPropertyAfterCommand = new ObservableCommand()
                .OnExecuted(x => Owner.NewPropertyAfter(this), Disposable)
                .AddTo(Disposable);

            RemovePropertyCommand = new ObservableCommand()
                .OnExecuted(x => Owner.RemoveProperty(this), Disposable)
                .AddTo(Disposable);

            CutPropertyCommand = new ObservableCommand()
                .OnExecuted(x => Owner.CutProperty(this), Disposable)
                .AddTo(Disposable);

            CopyPropertyCommand = new ObservableCommand()
                .OnExecuted(x => Owner.CopyProperty(this), Disposable)
                .AddTo(Disposable);

            PastePropertyCommand = new ObservableCommand()
                .AddPredicate(Clipboard.ObserveProperty(x => x.HasData, true))
                .AddPredicate(Clipboard.ObserveProperty(x => x.DataType, true).Select(x => x == typeof(XProperty)))
                .OnExecuted(x => Owner.PasteProperty(this), Disposable)
                .AddTo(Disposable);
        }

        public override void Dispose()
        {
            Disposable?.Dispose();
        }
    }
}
