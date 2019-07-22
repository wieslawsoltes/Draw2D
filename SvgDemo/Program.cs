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
        static void ResetColor()
        {
            Console.ResetColor();
        }

        static void WriteLine(string value, ConsoleColor color = ConsoleColor.Black)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }

        static void Print(SvgDocument doc)
        {
            WriteLine($"{doc}", ConsoleColor.Blue);
            Print(doc, "    ");
            ResetColor();
        }

        static void PrintAttributes(SvgElement element, string indent = "", string indentAttribute = "")
        {
            if (!string.IsNullOrEmpty(element.ID))
            {
                WriteLine($"{indent}{indentAttribute}[id]={element.ID}", ConsoleColor.Black);
            }

            // Attributes

            if (element.Fill != null && element.Fill != SvgPaintServer.None)
            {
                WriteLine($"{indent}{indentAttribute}[fill]={element.Fill.GetType()}", ConsoleColor.Black);
            }

            if (element.Stroke != null && element.Stroke != SvgPaintServer.None)
            {
                WriteLine($"{indent}{indentAttribute}[stroke]={element.Stroke.GetType()}", ConsoleColor.Black);
            }

            if (element.FillRule != SvgFillRule.NonZero)
            {
                WriteLine($"{indent}{indentAttribute}[fill-rule]={element.FillRule}", ConsoleColor.Black);
            }

            if (element.FillOpacity != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[fill-opacity]={element.FillOpacity}", ConsoleColor.Black);
            }

            if (element.StrokeWidth != 1f)
            {
                WriteLine($"{indent}{indentAttribute}[stroke-width]={element.StrokeWidth}", ConsoleColor.Black);
            }

            // ...


            // CustomAttributes

            if (element.CustomAttributes.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<CustomAttributes>", ConsoleColor.Gray);
                
                foreach (var attribute in element.CustomAttributes)
                {
                    WriteLine($"{indent}{indentAttribute}[{attribute.Key}]={attribute.Value}", ConsoleColor.Black);
                }
            }

            // Transforms

            if (element.Transforms.Count > 0)
            {
                WriteLine($"{indent}{indentAttribute}<Transforms>", ConsoleColor.Gray);
                WriteLine($"{indent}{indentAttribute}[transform]", ConsoleColor.Black);

                foreach (var transform in element.Transforms)
                {
                    WriteLine($"{indent}{indentAttribute}{transform}", ConsoleColor.Black);
                }
            }
        }

        static void Print(SvgElement element, string indent = "", string indentAttribute = "")
        {
            // Attributes

            PrintAttributes(element, indent, indentAttribute);

            // Children

            if (element.Children.Count > 0)
            {
                WriteLine($"{indent}<Children>", ConsoleColor.Gray);

                foreach (var child in element.Children)
                {
                    WriteLine($"{indent}{child}", ConsoleColor.Blue);
                    Print(child, indent + "    ", indentAttribute);
                }
            }

            // Nodes

            if (element.Nodes.Count > 0)
            {
                WriteLine($"{indent}<Nodes>", ConsoleColor.Gray);

                foreach (var node in element.Nodes)
                {
                    WriteLine($"{indent}{node.Content}", ConsoleColor.Black);
                }
            }
        }

        static void Run(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string path = args[i];
                WriteLine($"Path: {path}", ConsoleColor.Black);

                var doc = SvgDocument.Open<SvgDocument>(path, null);
                Print(doc);
            }
        }

        static void Error(Exception ex)
        {
            WriteLine($"{ex.Message}", ConsoleColor.Red);
            WriteLine($"{ex.StackTrace}", ConsoleColor.Black);
            if (ex.InnerException != null)
            {
                Error(ex.InnerException);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }
    }
}
