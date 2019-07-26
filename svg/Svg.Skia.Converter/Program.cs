// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SkiaSharp;
using Svg;

namespace Svg.Skia.Demo
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
            Console.WriteLine($"Usage: Svg.Skia.Demo -f <file.svg>");
            Console.WriteLine($"       Svg.Skia.Demo -d <indir> *.svg");
            Console.WriteLine($"       Svg.Skia.Demo -d <indir> *.svg <outdir>");
            Console.WriteLine($"       Svg.Skia.Demo -d . *.svg");
            Console.WriteLine($"       Svg.Skia.Demo -d . *.svg <outdir>");
        }

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Usage();
                    return;
                }

                var paths = new List<string>();
                var output = default(string);

                if (args[0] == "-f" && args.Length == 2)
                {
                    paths.Add(args[1]);
                }
                else if (args[0] == "-d" && (args.Length == 3 || args.Length == 4))
                {
                    var files = Directory.EnumerateFiles(args[1], args[2]);

                    paths.AddRange(files);

                    if (args.Length == 4)
                    {
                        output = args[3];
                    }
                }
                else
                {
                    Usage();
                    return;
                }

                var sw = Stopwatch.StartNew();

                foreach (var path in paths)
                {
                    try
                    {
                        var svgDocument = SvgDocument.Open<SvgDocument>(path, null);
                        if (svgDocument != null)
                        {
                            svgDocument.FlushStyles(true);

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

                            SkiaSvgRenderer.SaveImage(svgDocument, pngPath, SKEncodedImageFormat.Png, 100, 1, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Error(ex);
                    }
                }

                sw.Stop();
                Console.WriteLine($"Done: {sw.Elapsed}");
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
    }
}
