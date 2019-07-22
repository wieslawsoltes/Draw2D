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
        static void Print(IEnumerable<SvgElement> nodes, string indent = "")
        {
            foreach (var node in nodes)
            {
                Console.WriteLine($"{node}");
                //Console.WriteLine($"Fill: {node.Fill}");
                //Console.WriteLine($"Color: {node.Color}");
                //Console.WriteLine($"StopColor: {node.StopColor}");
                //Console.WriteLine($"Stroke: {node.Stroke}");

                Print(node.Descendants(), indent + "    ");
            }
        }

        static void Main(string[] args)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../ExportFile_16x.svg");

            Console.WriteLine($"Path: {path}");

            try
            {
                var doc = SvgDocument.Open<SvgDocument>(path, null);
                Print(doc.Descendants());
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
