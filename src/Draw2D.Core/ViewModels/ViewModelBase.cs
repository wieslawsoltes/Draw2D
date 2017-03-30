using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Draw2D.Models;

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
