using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace TwainScanner
{
    public class ImageConverter
    {
        public static void PpmToJpeg(string ppm, string jpeg, int quality)
        {
            bool grayscale = true;
            byte[] buffer = new byte[3];
            byte[] gray = { 0x50, 0x35, 0x0A };
            using (var fs = new FileStream(ppm, FileMode.Open, FileAccess.Read))
            {
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] != gray[i])
                {
                    grayscale = false;
                    break;
                }
            }

            var pluginsDir = AppDomain.CurrentDomain.BaseDirectory + "\\plugins";
            var program = new Executable
            {
                WorkingDirectory = pluginsDir,
                ProgramFileName = pluginsDir + "\\pnmtojpeg.exe",
                Arguments = "-maxmemory=128m -optimize -smooth=10 " + (grayscale ? "-grayscale " : "") +
                                "-quality=" + quality + " \"" + ppm + "\"",

                StandardOutputEncoding = Console.OutputEncoding,
                StandardOutputFileName = jpeg
            };

            program.Run();
        }

        public static void JpegToBmp(string jpeg, string bmp, int width = 0, int height = 0)
        {
            using (Stream bmpStream = File.Open(jpeg, FileMode.Open))
            {
                Bitmap bitmap;
                var image = Image.FromStream(bmpStream);
                if (width > 0 && height > 0)
                {
                    bitmap = ResizeImage(image, width, height);
                }
                else
                {
                    bitmap = new Bitmap(image);
                }
             
                bitmap.Save(bmp);
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}