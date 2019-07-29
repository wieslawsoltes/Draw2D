// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using SkiaSharp;
using Svg;

namespace Svg.Skia.Converter
{
    public class Program
    {
        public static void Error(Exception ex)
        {
            Console.WriteLine($"{ex.Message}", ConsoleColor.Yellow);
            Console.WriteLine($"{ex.StackTrace}", ConsoleColor.White);
            if (ex.InnerException != null)
            {
                Error(ex.InnerException);
            }
        }

        public static void Execute(string file, string directory, string output, string pattern, string format, int quality, float scaleX, float scaleY, bool debug, bool quiet)
        {
            try
            {
                var paths = new List<string>();

                if (file != null)
                {
                    paths.Add(file);
                }

                if (directory != null)
                {
                    var files = Directory.EnumerateFiles(directory, pattern);
                    if (files != null)
                    {
                        paths.AddRange(files);
                    }
                }

                if (!string.IsNullOrEmpty(output))
                {
                    if (!Directory.Exists(output))
                    {
                        Directory.CreateDirectory(output);
                    }
                }

                var sw = Stopwatch.StartNew();

                int success = 0;

                for (int i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    try
                    {
                        if (quiet == false)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"[{i}] File: {path}");
                        }

                        var extension = Path.GetExtension(path);
                        string imagePath = path.Remove(path.Length - extension.Length) + "." + format.ToLower();
                        if (!string.IsNullOrEmpty(output))
                        {
                            imagePath = Path.Combine(output, Path.GetFileName(imagePath));
                        }

                        var svg = new Svg();
                        var picture = svg.Load(path);
                        if (picture != null)
                        {
                            if (debug == true && svg.Document != null)
                            {
                                string ymlPath = path.Remove(path.Length - extension.Length) + ".yml";
                                if (!string.IsNullOrEmpty(output))
                                {
                                    ymlPath = Path.Combine(output, Path.GetFileName(ymlPath));
                                }
                                SvgDebug.Print(svg.Document, ymlPath);
                            }

                            if (Enum.TryParse<SKEncodedImageFormat>(format, true, out var skEncodedImageFormat))
                            {
                                svg.Save(imagePath, skEncodedImageFormat, quality, scaleX, scaleY);
                            }
                            else
                            {
                                throw new ArgumentException($"Invalid output image format.", nameof(format));
                            }
                        }

                        if (quiet == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[{i}] Succes: {imagePath}");
                        }
                        success++;
                    }
                    catch (Exception ex)
                    {
                        if (quiet == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"[{i}] Error: {path}");
                        }
                        Error(ex);
                    }
                }

                sw.Stop();

                if (quiet == false && paths.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Done: {sw.Elapsed} ({success}/{paths.Count})");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                if (quiet == false)
                {
                    Console.ResetColor();
                    Error(ex);
                }
            }
        }

        public static async Task<int> Main(string[] args)
        {
            var optionFile = new Option(new[] { "--file", "-f" }, "The relative or absolute path to the input file")
            {
                Argument = new Argument<string>(defaultValue: () => null)
            };

            var optionDirectory = new Option(new[] { "--directory", "-d" }, "The relative or absolute path to the input directory")
            {
                Argument = new Argument<string>(defaultValue: () => null)
            };

            var optionOutput = new Option(new[] { "--output", "-o" }, "The relative or absolute path to the output directory")
            {
                Argument = new Argument<string>(defaultValue: () => null)
            };

            var optionPattern = new Option(new[] { "--pattern", "-p" }, "The search string to match against the names of files in the input path")
            {
                Argument = new Argument<string>(defaultValue: () => "*.svg")
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

            rootCommand.Handler = CommandHandler.Create(typeof(Program).GetMethod(nameof(Execute)));

            return await rootCommand.InvokeAsync(args);
        }
    }
}
