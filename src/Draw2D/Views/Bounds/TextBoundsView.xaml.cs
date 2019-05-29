﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Draw2D.Views.Bounds
{
    public class TextBoundsView : UserControl
    {
        public TextBoundsView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
