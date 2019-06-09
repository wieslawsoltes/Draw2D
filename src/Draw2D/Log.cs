// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D
{
    internal static class Log
    {
#if DEBUG
        public static void WriteLine(string message)
            => System.Diagnostics.Debug.WriteLine(message);
#else
        public static void WriteLine(string message)
            => System.Console.WriteLine(message);
#endif
    }
}
