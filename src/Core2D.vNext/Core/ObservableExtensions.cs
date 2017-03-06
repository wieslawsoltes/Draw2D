using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;

namespace Core2D
{
    public static class ObservableExtensions
    {
        public static IObservable<TValue> ObserveProperty<T, TValue>(
            this T source, 
            Expression<Func<T, TValue>> propertyExpression) where T : INotifyPropertyChanged
        {
            return source.ObserveProperty(propertyExpression, false);
        }

        public static IObservable<TValue> ObserveProperty<T, TValue>(
            this T source, 
            Expression<Func<T, TValue>> propertyExpression, 
            bool observeInitialValue) where T : INotifyPropertyChanged
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var getter = propertyExpression.Compile();

            var observable = Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => (s, e) => h(e),
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Where(x => x.PropertyName == memberExpression.Member.Name)
                .Select(_ => getter(source));

            if (observeInitialValue)
            {
                return observable.Merge(Observable.Return(getter(source)));
            }

            return observable;
        }

        public static IObservable<string> ObservePropertyChanged<T>(this T source)
            where T : INotifyPropertyChanged
        {
            var observable = Observable.FromEvent<
                PropertyChangedEventHandler,
                PropertyChangedEventArgs>(
                    h => (s, e) => h(e),
                    h => source.PropertyChanged += h,
                    h => source.PropertyChanged -= h)
                .Select(x => x.PropertyName);

            return observable;
        }

        public static IObservable<Unit> ObserveCollectonChanged<T>(this T source)
            where T : INotifyCollectionChanged
        {
            var observable = Observable.FromEvent<
                NotifyCollectionChangedEventHandler,
                NotifyCollectionChangedEventArgs>(
                    h => (s, e) => h(e),
                    h => source.CollectionChanged += h,
                    h => source.CollectionChanged -= h)
                .Select(_ => new Unit());

            return observable;
        }

        public static IObservable<Unit> ObserveCollectonChanged<T>(
            this T source, 
            NotifyCollectionChangedAction collectionChangeAction) where T : INotifyCollectionChanged
        {
            var observable = Observable
                .FromEvent<
                    NotifyCollectionChangedEventHandler,
                    NotifyCollectionChangedEventArgs>(
                    h => (s, e) => h(e),
                    h => source.CollectionChanged += h,
                    h => source.CollectionChanged -= h)
                .Where(x => x.Action == collectionChangeAction)
                .Select(_ => new Unit());

            return observable;
        }
    }
}
