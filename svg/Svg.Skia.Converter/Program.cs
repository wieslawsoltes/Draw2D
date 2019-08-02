// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Svg.Skia.Converter
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var optionFile = new Option(new[] { "--file", "-f" }, "The relative or absolute path to the input file")
            {
                Argument = new Argument<FileInfo>(defaultValue: () => null)
            };

            var optionDirectory = new Option(new[] { "--directory", "-d" }, "The relative or absolute path to the input directory")
            {
                Argument = new Argument<DirectoryInfo>(defaultValue: () => null)
            };

            var optionOutput = new Option(new[] { "--output", "-o" }, "The relative or absolute path to the output directory")
            {
                Argument = new Argument<DirectoryInfo>(defaultValue: () => null)
            };

            var optionPattern = new Option(new[] { "--pattern", "-p" }, "The search string to match against the names of files in the input path")
            {
                Argument = new Argument<string>(defaultValue: () => null)
            };

            var optionFormat = new Option(new[] { "--format" }, "The output image format")
            {
                Argument = new Argument<string>(defaultValue: () => "png")
            };

            var optionQuality = new Option(new[] { "--quality" }, "The output image quality")
            {
                Argument = new Argument<int>(defaultValue: () => 100)
            };

            var optionScaleX = new Option(new[] { "--scaleX" }, "The output image horizontal scaling factor")
            {
                Argument = new Argument<float>(defaultValue: () => 1f)
            };

            var optionScaleY = new Option(new[] { "--scaleY" }, "The output image vertical scaling factor")
            {
                Argument = new Argument<float>(defaultValue: () => 1f)
            };

            var optionDebug = new Option(new[] { "--debug" }, "Write debug output to a file")
            {
                Argument = new Argument<bool>()
            };

            var optionQuiet = new Option(new[] { "--quiet" }, "Set verbosity level to quiet")
            {
                Argument = new Argument<bool>()
            };

            var rootCommand = new RootCommand()
            {
                Description = "Converts a svg file to an encoded bitmap image."
            };

            rootCommand.AddOption(optionFile);
            rootCommand.AddOption(optionDirectory);
            rootCommand.AddOption(optionOutput);
            rootCommand.AddOption(optionPattern);
            rootCommand.AddOption(optionFormat);
            rootCommand.AddOption(optionQuality);
            rootCommand.AddOption(optionScaleX);
            rootCommand.AddOption(optionScaleY);
            rootCommand.AddOption(optionDebug);
            rootCommand.AddOption(optionQuiet);

            rootCommand.Handler = CommandHandler.Create(typeof(Converter).GetMethod(nameof(Convert)));

            return await rootCommand.InvokeAsync(args);
        }
    }
}
