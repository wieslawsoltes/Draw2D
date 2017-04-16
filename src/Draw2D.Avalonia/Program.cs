// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Logging.Serilog;
using Serilog;

namespace Draw2D.Avalonia
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeLogging();

            var app = new App();
            AppBuilder.Configure(app)
                .UsePlatformDetect()
                .SetupWithoutStarting();
            app.Start();
        }

        static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}
