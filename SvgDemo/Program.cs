using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Svg;

namespace SvgDemo
{
    class Program
    {
        static void Print(SvgDocument doc)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"{doc}");
            Print(doc, "    ");
            Console.ResetColor();
        }

        static void Print(SvgElement element, string indent = "", string indentAttribute = "    ")
        {
            // Children

            if (element.Children.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{indent}<Children>");
            }

            foreach (var child in element.Children)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{indent}{child}, [id]={child.ID}");

                // Attributes

                if (child.Fill != null && child.Fill != SvgPaintServer.None)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{indent}{indentAttribute}[fill]={child.Fill.GetType()}");
                }

                if (child.Stroke != null && child.Stroke != SvgPaintServer.None)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{indent}{indentAttribute}[stroke]={child.Stroke.GetType()}");
                }

                if (child.FillRule != SvgFillRule.NonZero)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{indent}{indentAttribute}[fill-rule]={child.FillRule}");
                }

                if (child.FillOpacity != 1f)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{indent}{indentAttribute}[fill-opacity]={child.FillOpacity}");
                }

                if (child.StrokeWidth != 1f)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"{indent}{indentAttribute}[stroke-width]={child.StrokeWidth}");
                }

                // ...

                Print(child, indent + "    ");
            }

            // CustomAttributes

            if (element.CustomAttributes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{indent}<CustomAttributes>");
            }

            foreach (var attribute in element.CustomAttributes)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"{indent}[{attribute.Key}]={attribute.Value}");
            }

            // Transforms

            if (element.Transforms.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{indent}<Transforms>");
            }

            foreach (var transform in element.Transforms)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"{indent}{transform}");
            }

            // Nodes

            if (element.Nodes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"{indent}<Nodes>");
            }

            foreach (var node in element.Nodes)
            {
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine($"{indent}{node.Content}");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    string path = args[i];
                    Console.WriteLine($"Path: {path}");

                    var doc = SvgDocument.Open<SvgDocument>(path, null);
                    Print(doc);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine(ex.InnerException);
                        Console.WriteLine(ex.InnerException.StackTrace);
                    }
                }
            }
        }
    }
}
