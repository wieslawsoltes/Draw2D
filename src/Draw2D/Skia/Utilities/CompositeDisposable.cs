using System;
using System.Collections.Generic;

namespace Draw2D
{
    internal class CompositeDisposable : IDisposable
    {
        public IList<IDisposable> Disposables { get; private set; }

        public CompositeDisposable()
        {
            Disposables = new List<IDisposable>();
        }

        public void Dispose()
        {
            foreach (var disposable in Disposables)
            {
                disposable?.Dispose();
            }
            Disposables = null;
        }
    }
}
