// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Draw2D.Core
{
    public abstract class IdObject : ObservableObject
    {
        private Guid _id;
        private string _name;

        public Guid Id
        {
            get => _id;
            set => Update(ref _id, value);
        }

        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }
    }
}
