// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.Core;

namespace Draw2D.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }
    }
}
