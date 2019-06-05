// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D
{
    public static class Log
    {
        public static void WriteLine(string message)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#else
            System.Console.WriteLine(message);
#endif
        }
    }
}
