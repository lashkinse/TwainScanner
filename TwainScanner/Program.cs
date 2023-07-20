using System;
using System.IO;
using System.Linq;

namespace TwainScanner
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            int ret = 0;
            if (args.Length == 2)
            {
                try
                {
                    var ini = new Ini("TwainScanner.ini");
                  

                    var tp = new Scanner.TwainProperties
                    {
                        Directory = args[0],
                        Name = args[1],

                        SourceIndex = int.Parse(ini.GetValue("SourceIndex", "Global", "0")),
                        DPI = int.Parse(ini.GetValue("DPI", "Global", "300")),
                        PixelType = ini.GetValue("PixelType", "Global", "BW"),
                        ShowUI = bool.Parse(ini.GetValue("ShowUI", "Global", "false"))
                    };
                    Scanner.Twain.Acquire(tp);

                    //Размер конечного изображения
                    var width = int.Parse(ini.GetValue("ImageWidth", "Global", "790"));
                    var height = int.Parse(ini.GetValue("ImageHeight", "Global", "1117"));

                    var ppms = Directory.GetFiles(tp.Directory, "*.ppm").OrderBy(file => new FileInfo(file).CreationTime);
                    foreach (var ppm in ppms)
                    {
                       
                        var jpeg = ppm.Replace(".ppm", ".jpg");
                        var bmp = ppm.Replace(".ppm", ".bmp");

                        ImageConverter.PpmToJpeg(ppm, jpeg, int.Parse(ini.GetValue("ImageQuality", "Global", "95")));

                        //ImageConverter.JpegToBmp(jpeg, bmp);
                        ImageConverter.JpegToBmp(jpeg, bmp, width, height); //изменяем размер и конвертируем

                        //Удаляем временные файлы
                        File.Delete(ppm);
                        File.Delete(jpeg);

                        Console.WriteLine(bmp); //пишем ПВД, что получилось
                    }
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Console.WriteLine(ex.Message);
                    ret = 4;
                }
            }
            else
            {
                Log(@"Usage: TwainScanner.exe Path FileName");
                Console.WriteLine(@"Usage: TwainScanner.exe Path FileName");
                ret = 1;
            }
            return ret;
        }

        private static void Log(string message)
        {
            File.AppendAllText("TwainScanner.log", DateTime.Now + @" " + message + Environment.NewLine);
        }

    }
}
