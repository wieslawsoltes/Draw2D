// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using SkiaSharp;
using Svg;

namespace Svg.Skia.Converter
{
    public class Converter
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(Exception ex)
        {
            Log($"{ex.Message}");
            Log($"{ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Error(ex.InnerException);
            }
        }

        public static bool Save(FileInfo path, DirectoryInfo output, string format, int quality, float scaleX, float scaleY, bool debug, bool quiet, int i)
        {
            try
            {
                if (quiet == false)
                {
                    Log($"[{i}] File: {path}");
                }

                var extension = path.Extension;
                string imagePath = path.FullName.Remove(path.FullName.Length - extension.Length) + "." + format.ToLower();
                if (!string.IsNullOrEmpty(output.FullName))
                {
                    imagePath = Path.Combine(output.FullName, Path.GetFileName(imagePath));
                }

                using (var svg = new Svg())
                {
                    SKPicture picture;

                    switch (path.Extension.ToLower())
                    {
                        default:
                        case ".svg":
                            {
                                picture = svg.Load(path.FullName);
                            }
                            break;
                        case ".svgz":
                            {
                                using (var fileStream = path.OpenRead())
                                using (var memoryStream = new MemoryStream())
                                using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                                {
                                    gzipStream.CopyTo(memoryStream);
                                    memoryStream.Position = 0;
                                    picture = svg.Load(memoryStream);
                                }
                            }
                            break;
                    }

                    if (picture != null)
                    {
                        if (debug == true && svg.Document != null)
                        {
                            string ymlPath = path.FullName.Remove(path.FullName.Length - extension.Length) + ".yml";
                            if (!string.IsNullOrEmpty(output.FullName))
                            {
                                ymlPath = Path.Combine(output.FullName, Path.GetFileName(ymlPath));
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
                }

                if (quiet == false)
                {
                    Log($"[{i}] Success: {imagePath}");
                }

                return true;
            }
            catch (Exception ex)
            {
                if (quiet == false)
                {
                    Log($"[{i}] Error: {path}");
                    Error(ex);
                }
            }

            return false;
        }

        public static void GetFiles(DirectoryInfo directory, string pattern, List<FileInfo> paths)
        {
            var files = Directory.EnumerateFiles(directory.FullName, pattern);
            if (files != null)
            {
                foreach (var path in files)
                {
                    paths.Add(new FileInfo(path));
                }
            }
        }

        public static void Convert(FileInfo file, DirectoryInfo directory, DirectoryInfo output, string pattern, string format, int quality, float scaleX, float scaleY, bool debug, bool quiet)
        {
            try
            {
                var paths = new List<FileInfo>();

                if (file != null)
                {
                    paths.Add(file);
                }

                if (directory != null)
                {
                    if (pattern == null)
                    {
                        GetFiles(directory, "*.svg", paths);
                        GetFiles(directory, "*.svgz", paths);
                    }
                    else
                    {
                        GetFiles(directory, pattern, paths);
                    }
                }

                if (!string.IsNullOrEmpty(output.FullName))
                {
                    if (!Directory.Exists(output.FullName))
                    {
                        Directory.CreateDirectory(output.FullName);
                    }
                }

                var sw = Stopwatch.StartNew();

                int processed = 0;

                for (int i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];

                    if (Save(path, output, format, quality, scaleX, scaleY, debug, quiet, i))
                    {
                        processed++;
                    }
                }

                sw.Stop();

                if (quiet == false && paths.Count > 0)
                {
                    Log($"Done: {sw.Elapsed} ({processed}/{paths.Count})");
                }
            }
            catch (Exception ex)
            {
                if (quiet == false)
                {
                    Error(ex);
                }
            }
        }
    }

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
