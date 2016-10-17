// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;

namespace Core2D.History
{
    /// <summary>
    /// Undo/redo stack based action history.
    /// </summary>
    public sealed class StackHistory : ObservableObject, IHistory
    {
        public ImmutableStack<State> _undoStates;
        public ImmutableStack<State> _redoStates;
        public bool _canUndo;
        public bool _canRedo;
        public bool _canClear;

        public ImmutableStack<State> UndoStates
        {
            get { return _undoStates; }
            set { Update(ref _undoStates, value); }
        }

        public ImmutableStack<State> RedoStates
        {
            get { return _redoStates; }
            set { Update(ref _redoStates, value); }
        }

        public bool CanUndo
        {
            get { return _canUndo; }
            set { Update(ref _canUndo, value); }
        }

        public bool CanRedo
        {
            get { return _canRedo; }
            set { Update(ref _canRedo, value); }
        }

        public bool CanClear
        {
            get { return _canClear; }
            set { Update(ref _canClear, value); }
        }

        public StackHistory()
        {
            _undoStates = ImmutableStack.Create<State>();
            _redoStates = ImmutableStack.Create<State>();
            _canUndo = false;
            _canRedo = false;
            _canClear = false;
        }

        /// <inheritdoc/>
        State IHistory.Snapshot<T>(T previous, T next, Action<T> update, string name)
        {
            var state = State.Create(() => update(previous), () => update(next), name);

            if (!RedoStates.IsEmpty)
            {
                RedoStates = RedoStates.Clear();
                CanRedo = false;
            }

            UndoStates = UndoStates.Push(state);
            CanUndo = true;

            CanClear = CanUndo | CanRedo;

            return state;
        }

        /// <inheritdoc/>
        bool IHistory.Undo()
        {
            if (UndoStates.IsEmpty)
            {
                return false;
            }

            State state;
            UndoStates = UndoStates.Pop(out state);
            state.ToPrevious();

            if (UndoStates.IsEmpty)
            {
                CanUndo = false;
            }

            RedoStates = RedoStates.Push(state);
            CanRedo = true;

            CanClear = CanUndo | CanRedo;

            return true;
        }

        /// <inheritdoc/>
        bool IHistory.Redo()
        {
            if (RedoStates.IsEmpty)
            {
                return false;
            }

            State state;
            RedoStates = RedoStates.Pop(out state);
            state.ToNext();

            if (RedoStates.IsEmpty)
            {
                CanRedo = false;
            }

            UndoStates = UndoStates.Push(state);
            CanUndo = true;

            CanClear = CanUndo | CanRedo;

            return true;
        }

        /// <inheritdoc/>
        void IHistory.Clear()
        {
            UndoStates = UndoStates.Clear();
            RedoStates = RedoStates.Clear();
            CanUndo = false;
            CanRedo = false;
            CanClear = false;
        }
    }
}
