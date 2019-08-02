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
            var optionFile = new Option(new[] { "--files", "-f" }, "The relative or absolute path to the input files")
            {
                Argument = new Argument<FileInfo[]>(defaultValue: () => null)
            };

            var optionDirectory = new Option(new[] { "--directories", "-d" }, "The relative or absolute path to the input directories")
            {
                Argument = new Argument<DirectoryInfo[]>(defaultValue: () => null)
            };

            var optionOutput = new Option(new[] { "--output", "-o" }, "The relative or absolute path to the output directory")
            {
                Argument = new Argument<DirectoryInfo>(defaultValue: () => null)
            };

            var optionPattern = new Option(new[] { "--pattern", "-p" }, "The search string to match against the names of files in the input directory")
            {
                Argument = new Argument<string>(defaultValue: () => null)
            };

            var optionFormat = new Option(new[] { "--format" }, "The output image format")
            {
                Argument = new Argument<string>(defaultValue: () => "png")
            };

            var optionQuality = new Option(new[] { "--quality", "-q" }, "The output image quality")
            {
                Argument = new Argument<int>(defaultValue: () => 100)
            };

            var optionBackground = new Option(new[] { "--background", "-b" }, "The output image background")
            {
                Argument = new Argument<string>(defaultValue: () => "#00000000")
            };

            var optionScale = new Option(new[] { "--scale", "-s" }, "The output image horizontal and vertical scaling factor")
            {
                Argument = new Argument<float>(defaultValue: () => 1f)
            };

            var optionScaleX = new Option(new[] { "--scaleX", "-sx" }, "The output image horizontal scaling factor")
            {
                Argument = new Argument<float>(defaultValue: () => 1f)
            };

            var optionScaleY = new Option(new[] { "--scaleY", "-sy" }, "The output image vertical scaling factor")
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
            rootCommand.AddOption(optionBackground);
            rootCommand.AddOption(optionScale);
            rootCommand.AddOption(optionScaleX);
            rootCommand.AddOption(optionScaleY);
            rootCommand.AddOption(optionDebug);
            rootCommand.AddOption(optionQuiet);

            rootCommand.Handler = CommandHandler.Create((ConverterSettings settings) => Converter.Convert(settings));

            return await rootCommand.InvokeAsync(args);
        }
    }
}
