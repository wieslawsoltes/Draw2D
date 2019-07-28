// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Text;
using SkiaSharp;
using Svg;

namespace Svg.Skia.Converter
{
    class Program
    {
        static void Error(Exception ex)
        {
            Console.WriteLine($"{ex.Message}", ConsoleColor.Yellow);
            Console.WriteLine($"{ex.StackTrace}", ConsoleColor.Black);
            if (ex.InnerException != null)
            {
                Error(ex.InnerException);
            }
        }

        static void Usage()
        {
            Console.WriteLine($"Usage: Svg.Skia.Converter -f path [-o path]");
            Console.WriteLine($"       Svg.Skia.Converter -d path [-o path]");
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 2 && args.Length != 4)
                {
                    Usage();
                    return;
                }

                var paths = new List<string>();
                var output = default(string);

                if (args.Length == 4)
                {
                    if (args[2] == "-o")
                    {
                        output = args[3];
                    }
                    else
                    {
                        Usage();
                        return; 
                    }
                }

                switch (args[0])
                {
                    case "-f":
                        {
                            paths.Add(args[1]);
                        }
                        break;
                        case "-d":
                        {
                            var files = Directory.EnumerateFiles(args[1], "*.svg");
                            if (files != null)
                            {
                                paths.AddRange(files);
                            }
                        }
                        break;
                    default:
                        {
                            Usage();
                            return;
                        }
                }

                var sw = Stopwatch.StartNew();

                int success = 0;
                int errors = 0;

                if (!string.IsNullOrEmpty(output))
                {
                    if (!Directory.Exists(output))
                    {
                        Directory.CreateDirectory(output);
                    }    
                }

                foreach (var path in paths)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"File: {path}");

                        var svg = new Svg();
                        var picture = svg.Load(path);
                        if (picture != null)
                        {
                            var extension = Path.GetExtension(path);
#if false
                        var svgDebug = new SvgDebug()
                        {
                            Builder = new StringBuilder(),
                            IndentTab = "  ",
                            PrintSvgElementAttributesEnabled = true,
                            PrintSvgElementCustomAttributesEnabled = true,
                            PrintSvgElementChildrenEnabled = true,
                            PrintSvgElementNodesEnabled = false
                        };
                        svgDebug.PrintSvgElement(svgDocument, "", "");
                        if (svgDebug.Builder != null)
                        {
                            var yaml = svgDebug.Builder.ToString();
                            string ymlPath = path.Remove(path.Length - extension.Length) + ".yml";
                            if (!string.IsNullOrEmpty(output))
                            {
                                ymlPath = Path.Combine(output, Path.GetFileName(ymlPath));
                            }
                            File.WriteAllText(ymlPath, yaml);
                        }
#endif
                            string pngPath = path.Remove(path.Length - extension.Length) + ".png";
                            if (!string.IsNullOrEmpty(output))
                            {
                                pngPath = Path.Combine(output, Path.GetFileName(pngPath));
                            }
                            svg.Save(pngPath, SKEncodedImageFormat.Png, 100, 1, 1);
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Succes: {path}");
                        success++;
                    }
                    catch (Exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error: {path}");
                        errors++;
                    }
                }

                sw.Stop();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"Done: {sw.Elapsed} ({success}/{paths.Count})");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ResetColor();
                Error(ex);
            }
        }
    }
}
