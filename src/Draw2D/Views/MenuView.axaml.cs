﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views
{
    public class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
