// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;

namespace Core2D.History
{
    /// <summary>
    /// Undo/redo action history contract.
    /// </summary>
    public interface IHistory : INotifyPropertyChanged
    {
        /// <summary>
        /// Makes undo/redo history snapshot.
        /// </summary>
        /// <param name="previous">The previous state.</param>
        /// <param name="next">The next state.</param>
        /// <param name="update">The update method.</param>
        /// <param name="name">The snapshot name.</param>
        /// <typeparam name="T">The state object type.</typeparam>
        /// <returns>The undo/redo action pair.</returns>
        State Snapshot<T>(T previous, T next, Action<T> update, string name);

        /// <summary>
        /// Gets or sets flag indicating whether undo action can execute.
        /// </summary>
        bool CanUndo { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether redo action can execute.
        /// </summary>
        bool CanRedo { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether clear action can execute.
        /// </summary>
        bool CanClear { get; set; }

        /// <summary>
        /// Executes undo action.
        /// </summary>
        /// <returns>True if undo action was executed.</returns>
        bool Undo();

        /// <summary>
        /// Executes redo action.
        /// </summary>
        /// <returns>True if redo action was executed.</returns>
        bool Redo();

        /// <summary>
        /// Clears undo/redo actions history.
        /// </summary>
        void Clear();
    }
}
