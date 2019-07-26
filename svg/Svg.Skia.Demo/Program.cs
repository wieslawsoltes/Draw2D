using System;
using System.Collections.Generic;
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

        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine($"Usage: SvgDemo -f <filename.svg>");
                    Console.WriteLine($"       SvgDemo -d ./ *.svg");
                    return;
                }

                var paths = new List<string>();

                if (args[0] == "-f" && args.Length == 2)
                {
                    paths.Add(args[1]);
                }
                else if (args[0] == "-d" && args.Length == 2)
                {
                    paths.AddRange(Directory.EnumerateFiles(".", args[1]));
                }
                else if (args[0] == "-d" && args.Length == 3)
                {
                    paths.AddRange(Directory.EnumerateFiles(args[1], args[2]));
                }
                else
                {
                    Console.WriteLine($"Usage: SvgDemo -f <filename.svg>");
                    Console.WriteLine($"       SvgDemo -d *.svg");
                    Console.WriteLine($"       SvgDemo -d . *.svg");
                    return;
                }

                foreach (var path in paths)
                {
                    var svgDocument = SvgDocument.Open<SvgDocument>(path, null);
                    if (svgDocument != null)
                    {
                        svgDocument.FlushStyles(true);

                        var extension = Path.GetExtension(path).ToCharArray();

                        //var svgDebug = new SvgDebug()
                        //{
                        //    Builder = new StringBuilder(),
                        //    IndentTab = "  ",
                        //    PrintSvgElementAttributesEnabled = true,
                        //    PrintSvgElementCustomAttributesEnabled = true,
                        //    PrintSvgElementChildrenEnabled = true,
                        //    PrintSvgElementNodesEnabled = false
                        //};
                        //svgDebug.PrintSvgElement(svgDocument, "", "");
                        //if (svgDebug.Builder != null)
                        //{
                        //    var yaml = svgDebug.Builder.ToString();
                        //    string ymlPath = path.TrimEnd(extension) + ".yml";
                        //    File.WriteAllText(ymlPath, yaml);
                        //}

                        string pngPath = path.TrimEnd(extension) + ".png";
                        SkiaSvgRenderer.SaveImage(svgDocument, pngPath, SKEncodedImageFormat.Png, 100, 1, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Error(ex);
                Console.ResetColor();
            }
        }
    }
}
