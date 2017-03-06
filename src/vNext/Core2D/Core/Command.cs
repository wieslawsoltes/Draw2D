// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;

namespace Core2D
{
    /// <summary>
    /// Input command.
    /// </summary>
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        public Command(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="execute">The execute action.</param>
        /// <param name="canExecute">The can execute predicate.</param>
        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Check if can invoke execute action.
        /// </summary>
        /// <param name="parameter">The can execute parameter.</param>
        /// <returns>True if can invoke execute action.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Gets or sets CanExecuteChanged event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /*
        /// <summary>
        /// Gets or sets CanExecuteChanged event handler.
        /// </summary>
        public event EventHandler CanExecuteChanged;
        */

        /// <summary>
        /// Invoke execute action.
        /// </summary>
        /// <param name="parameter">The execute parameter.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /*
        /// <summary>
        /// Raise <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        /// <summary>
        /// Raise <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        */
    }
}
