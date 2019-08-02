// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                    if (svg.Load(path.FullName) != null)
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
}
