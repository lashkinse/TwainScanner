using System;
using System.Collections.Generic;
using System.IO;

namespace TwainScanner
{
    public class Scanner
    {
        public class TwainProperties
        {
            public static class Color
            {
                public const string BW = "BW";
                public const string RGB = "RGB";
            }

            public int SourceIndex { get; set; }
            public int DPI { get; set; }
            public string PixelType { get; set; }
            public bool ShowUI { get; set; }

            public string Directory
            {
                get
                {
                    if (!System.IO.Directory.Exists(_directory))
                    {
                        System.IO.Directory.CreateDirectory(_directory);
                    }

                    return _directory;
                }
                set { _directory = value; }
            }
            private string _directory;

            public string Name { get; set; }
        }

        public static class Twain
        {
            public static void Acquire(TwainProperties tp)
            {
                // /source 1 /ui 0 /dpi 300 /color BW /dir "C:\Users\admin1\AppData\Local\Temp\Kite" /file scan

                var pluginsDir = AppDomain.CurrentDomain.BaseDirectory + "\\plugins";
                var program = new Executable
                {
                    WorkingDirectory = pluginsDir,
                    ProgramFileName = pluginsDir + "\\Saraff.Twain.Acquire.exe",
                    Arguments = "/source " + tp.SourceIndex +
                                " /ui " + (tp.ShowUI ? 1 : 0) +
                                (tp.DPI == 0 ? "" : " /dpi " + tp.DPI) +
                                (string.IsNullOrEmpty(tp.PixelType) ? "" : " /color " + tp.PixelType) +
                                " /dir " + tp.Directory +
                                " /file " + tp.Name
                };
                Console.WriteLine(program.Arguments);
                program.Run();
            }
        }
    }
}