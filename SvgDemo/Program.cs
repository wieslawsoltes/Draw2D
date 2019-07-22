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
            Console.WriteLine($"{doc}");
            Print(doc, "    ");
        }

        static void Print(SvgElement element, string indent = "")
        {
            // Attributes

            if (element.CustomAttributes.Count > 0)
            {
                Console.WriteLine($"{indent}<CustomAttributes>");
            }

            foreach (var attribute in element.CustomAttributes)
            {
                Console.WriteLine($"{indent}[{attribute.Key}]={attribute.Value}");
            }

            // Transforms

            if (element.Transforms.Count > 0)
            {
                Console.WriteLine($"{indent}<Transforms>");
            }

            foreach (var transform in element.Transforms)
            {
                Console.WriteLine($"{indent}{transform}");
            }

            // Children

            if (element.Children.Count > 0)
            {
                Console.WriteLine($"{indent}<Children>");
            }

            foreach (var child in element.Children)
            {
                Console.WriteLine($"{indent}{child}, [id]={child.ID}");

                //Console.WriteLine($"Fill: {child.Fill}");
                //Console.WriteLine($"Color: {child.Color}");
                //Console.WriteLine($"StopColor: {child.StopColor}");
                //Console.WriteLine($"Stroke: {child.Stroke}");

                Print(child, indent + "    ");
            }

            // Nodes

            if (element.Nodes.Count > 0)
            {
                Console.WriteLine($"{indent}<Nodes>");
            }

            foreach (var node in element.Nodes)
            {
                Console.WriteLine($"{indent}{node.Content}");
            }
        }

        static void Main(string[] args)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../ExportFile_16x.svg");

            Console.WriteLine($"Path: {path}");

            try
            {
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
