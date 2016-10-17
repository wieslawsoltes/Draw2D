// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.History
{
    /// <summary>
    /// Undo/redo action pair.
    /// </summary>
    public struct State
    {
        /// <summary>
        /// The undo action.
        /// </summary>
        public Action ToPrevious { get; }

        /// <summary>
        /// The redo action.
        /// </summary>
        public Action ToNext { get; }

        /// <summary>
        /// The state name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new <see cref="State"/> instance.
        /// </summary>
        /// <param name="previous">The previous state update method.</param>
        /// <param name="next">The next state update method.</param>
        /// <param name="name">The state name.</param>
        public State(Action previous, Action next, string name)
        {
            ToPrevious = previous;
            ToNext = next;
            Name = name;
        }

        /// <summary>
        /// Creates a new <see cref="State"/> instance.
        /// </summary>
        /// <param name="previous">The previous state update method.</param>
        /// <param name="next">The next state update method.</param>
        /// <param name="name">The state name.</param>
        /// <returns>The new instance of <see cref="State"/> object.</returns>
        public static State Create(Action previous, Action next, string name)
        {
            return new State(previous, next, name);
        }
    }
}
