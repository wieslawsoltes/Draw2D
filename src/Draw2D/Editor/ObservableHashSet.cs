using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Draw2D
{
    internal static class EnumerableExtensions
    {
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item)
        {
            return source.IndexOf<TSource>(item, null);
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource item, IEqualityComparer<TSource> itemComparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                return list.IndexOf(item);
            }
            IList list2 = source as IList;
            if (list2 != null)
            {
                return list2.IndexOf(item);
            }
            if (itemComparer == null)
            {
                itemComparer = EqualityComparer<TSource>.Default;
            }
            int num = 0;
            foreach (TSource local in source)
            {
                if (itemComparer.Equals(item, local))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }
    }

    /// <summary>
    /// Represents an observable set of values.
    /// </summary>
    /// <typeparam name="T">The type of elements in the hash set.</typeparam>
    internal class ObservableHashSet<T> : ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        private readonly HashSet<T> _hashSet;
        private SimpleMonitor _monitor;

        /// <summary>
        /// Raised when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet`1" /> class.
        /// </summary>
        public ObservableHashSet()
        {
            this._monitor = new SimpleMonitor();
            this._hashSet = new HashSet<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet`1" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        public ObservableHashSet(IEnumerable<T> collection)
        {
            this._monitor = new SimpleMonitor();
            this._hashSet = new HashSet<T>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet`1" /> class.
        /// </summary>
        /// <param name="comparer">The IEqualityComparer&lt;T&gt; implementation to use when comparing values in the set, or null to use the default EqualityComparer&lt;T&gt; implementation for the set type.</param>
        public ObservableHashSet(IEqualityComparer<T> comparer)
        {
            this._monitor = new SimpleMonitor();
            this._hashSet = new HashSet<T>(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashSet`1" /> class.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The IEqualityComparer&lt;T&gt; implementation to use when comparing values in the set, or null to use the default EqualityComparer&lt;T&gt; implementation for the set type.</param>
        public ObservableHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer)
        {
            this._monitor = new SimpleMonitor();
            this._hashSet = new HashSet<T>(collection, comparer);
        }

        /// <summary>
        /// Adds the specified element to a set.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>true if the element is added to the <see cref="ObservableHashSet`1" /> object; false if the element is already present.</returns>
        public bool Add(T item)
        {
            // TODO:
            //this.CheckReentrancy();
            bool flag = this._hashSet.Add(item);
            if (flag)
            {
                // TODO:
                //int index = this._hashSet.IndexOf<T>(item);
                //this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
                //this.RaisePropertyChanged("Count");
            }
            return flag;
        }

        private IDisposable BlockReentrancy()
        {
            this._monitor.Enter();
            return this._monitor;
        }

        private void CheckReentrancy()
        {
            if ((this._monitor.Busy && (this.CollectionChanged != null)) && (this.CollectionChanged.GetInvocationList().Length > 1))
            {
                throw new InvalidOperationException("There are additional attempts to change this hash set during a CollectionChanged event.");
            }
        }

        /// <summary>
        /// Removes all elements from a <see cref="ObservableHashSet`1" /> object.
        /// </summary>
        public void Clear()
        {
            // TODO:
            //this.CheckReentrancy();
            if (this._hashSet.Count > 0)
            {
                this._hashSet.Clear();
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                this.RaisePropertyChanged("Count");
            }
        }

        /// <summary>
        /// Determines whether a <see cref="ObservableHashSet`1" /> object contains the specified element.
        /// </summary>
        /// <param name="item">The element to locate in the <see cref="ObservableHashSet`1" /> object.</param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object contains the specified element; otherwise, false.</returns>
        public bool Contains(T item) => this._hashSet.Contains(item);

        /// <summary>
        /// Copies the elements of a <see cref="ObservableHashSet`1" /> collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="ObservableHashSet`1" /> object. The array must have zero-based indexing.</param>
        public void CopyTo(T[] array) => this._hashSet.CopyTo(array);

        /// <summary>
        /// Copies the elements of a <see cref="ObservableHashSet`1" /> collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="ObservableHashSet`1" /> object. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex) => this._hashSet.CopyTo(array, arrayIndex);

        /// <summary>
        /// Copies the elements of a <see cref="ObservableHashSet`1" /> collection to an array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the <see cref="ObservableHashSet`1" /> object. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <param name="count">The number of elements to copy to array.</param>
        public void CopyTo(T[] array, int arrayIndex, int count) => this._hashSet.CopyTo(array, arrayIndex, count);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this._monitor != null)
            {
                this._monitor.Dispose();
                this._monitor = null;
            }
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current <see cref="ObservableHashSet`1" /> object.
        /// </summary>
        /// <param name="other">The collection of items to remove from the <see cref="ObservableHashSet`1" /> object.</param>
        public void ExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            // TODO:
            //this.CheckReentrancy();
            List<T> changedItems = (from x in other
                                    where _hashSet.Contains(x)
                                    select x).ToList<T>();
            this._hashSet.ExceptWith(other);
            if (changedItems.Count > 0)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems));
                this.RaisePropertyChanged("Count");
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a <see cref="ObservableHashSet`1" />.
        /// </summary>
        /// <returns>A <see cref="ObservableHashSet`1" />.Enumerator object for the <see cref="ObservableHashSet`1" /> object.</returns>
        public IEnumerator<T> GetEnumerator() => this._hashSet.GetEnumerator();

        /// <summary>
        /// Modifies the current <see cref="ObservableHashSet`1" /> object to contain only elements that are present in that object and in the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object.</param>
        public void IntersectWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            // TODO:
            //this.CheckReentrancy();
            List<T> changedItems = (from x in this._hashSet
                                    where !other.Contains<T>(x)
                                    select x).ToList<T>();
            this._hashSet.IntersectWith(other);
            if (changedItems.Count > 0)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems));
                this.RaisePropertyChanged("Count");
            }
        }

        /// <summary>
        /// Determines whether a <see cref="ObservableHashSet`1" /> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object.</param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object is a proper subset of other; otherwise, false.</returns>
        public bool IsProperSubsetOf(IEnumerable<T> other) => this._hashSet.IsProperSubsetOf(other);

        /// <summary>
        /// Determines whether a <see cref="ObservableHashSet`1" /> object is a proper subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object. </param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object is a proper superset of other; otherwise, false.</returns>
        public bool IsProperSupersetOf(IEnumerable<T> other) => this._hashSet.IsProperSupersetOf(other);

        /// <summary>
        /// Determines whether a <see cref="ObservableHashSet`1" /> object is a subset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object. </param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object is a subset of other; otherwise, false.</returns>
        public bool IsSubsetOf(IEnumerable<T> other) => this._hashSet.IsSubsetOf(other);

        /// <summary>
        /// Determines whether a <see cref="ObservableHashSet`1" /> object is a superset of the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object. </param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object is a superset of other; otherwise, false.</returns>
        public bool IsSupersetOf(IEnumerable<T> other) => this._hashSet.IsSupersetOf(other);

        /// <summary>
        /// Determines whether the current <see cref="ObservableHashSet`1" /> object and a specified collection share common elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object. </param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object and other share at least one common element; otherwise, false.</returns>
        public bool Overlaps(IEnumerable<T> other) => this._hashSet.Overlaps(other);

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                using (this.BlockReentrancy())
                {
                    this.CollectionChanged(this, e);
                }
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Removes the specified element from a <see cref="ObservableHashSet`1" /> object.
        /// </summary>
        /// <param name="item">The element to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false. This method returns false if item is not found in the <see cref="ObservableHashSet`1" /> object.</returns>
        public bool Remove(T item)
        {
            int index = this._hashSet.IndexOf<T>(item);
            bool flag = this._hashSet.Remove(item);
            if (flag)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                this.RaisePropertyChanged("Count");
            }
            return flag;
        }

        public int RemoveWhere(Predicate<T> match)
        {
            List<T> list = (from m in this._hashSet
                            where match(m)
                            select m).ToList<T>();
            foreach (T local in list)
            {
                this.Remove(local);
            }
            return list.Count;
        }

        /// <summary>
        /// Determines whether a <see cref="ObservableHashSet`1" /> object and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object. </param>
        /// <returns>true if the <see cref="ObservableHashSet`1" /> object is equal to other; otherwise, false.</returns>
        public bool SetEquals(IEnumerable<T> other) => this._hashSet.SetEquals(other);

        /// <summary>
        /// Modifies the current <see cref="ObservableHashSet`1" /> object to contain only elements that are present either in that object or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object.</param>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            // TODO:
            //this.CheckReentrancy();
            List<T> changedItems = (from x in other
                                    where !_hashSet.Contains(x)
                                    select x).ToList<T>();
            List<T> list2 = (from x in other
                             where _hashSet.Contains(x)
                             select x).ToList<T>();
            this._hashSet.SymmetricExceptWith(other);
            if (list2.Count > 0)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, list2));
                this.RaisePropertyChanged("Count");
            }
            if (changedItems.Count > 0)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems));
            }
            if ((list2.Count > 0) || (changedItems.Count > 0))
            {
                this.RaisePropertyChanged("Count");
            }
        }

        void ICollection<T>.Add(T item) => this.Add(item);

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this._hashSet.GetEnumerator();

        /// <summary>
        /// Sets the capacity of a <see cref="ObservableHashSet`1" /> object to the actual number of elements it contains, rounded up to a nearby, implementation-specific value.
        /// </summary>
        public void TrimExcess() => this._hashSet.TrimExcess();

        /// <summary>
        /// Modifies the current <see cref="ObservableHashSet`1" /> object to contain all elements that are present in itself, the specified collection, or both.
        /// </summary>
        /// <param name="other">The collection to compare to the current <see cref="ObservableHashSet`1" /> object.</param>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            // TODO:
            //this.CheckReentrancy();
            List<T> changedItems = (from x in other
                                    where !_hashSet.Contains(x)
                                    select x).ToList<T>();
            this._hashSet.UnionWith(other);
            if (changedItems.Count > 0)
            {
                this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems));
                this.RaisePropertyChanged("Count");
            }
        }

        /// <summary>
        /// Gets the IEqualityComparer&lt;T&gt; object that is used to determine equality for the values in the set.
        /// </summary>
        public IEqualityComparer<T> Comparer => this._hashSet.Comparer;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ObservableHashSet`1" />.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ObservableHashSet`1" />.
        /// </returns>
        public int Count => this._hashSet.Count;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)this._hashSet).IsReadOnly;

        /// <summary>
        /// The property names used with INotifyPropertyChanged.
        /// </summary>
        public static class PropertyNames
        {
            public const string Count = "Count";
            public const string IsReadOnly = "IsReadOnly";
        }

        private class SimpleMonitor : IDisposable
        {
            private int _busyCount;

            public void Dispose() => this._busyCount--;

            public void Enter() => this._busyCount++;

            public bool Busy => this._busyCount > 0;
        }
    }
}
