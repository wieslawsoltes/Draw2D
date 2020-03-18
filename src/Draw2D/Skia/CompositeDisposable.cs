// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
