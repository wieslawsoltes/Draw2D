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
        static void Print(IEnumerable<SvgElement> nodes)
        {
            foreach (var node in nodes)
            {
                //node.Fill
                //node.Color
                //node.StopColor
                //node.Stroke

                Console.WriteLine($"{node}");

                Print(node.Descendants());
            }
        }

        static void Main(string[] args)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"../../../ExportFile_16x.svg");

            Console.WriteLine($"Path: {path}");

            try
            {
                var d = new SvgDocument();

                //SvgDocument.EnsureSystemIsGdiPlusCapable();

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
