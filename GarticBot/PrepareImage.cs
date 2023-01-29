using System;
using System.Drawing;
using ColorHelper;

namespace GarticBot
{
    internal class PrepareImage
    {
        Color[] GarticColor = new[]
        {
            Color.FromArgb(0, 0, 0),
            Color.FromArgb(255, 255, 255),
            Color.FromArgb(1, 116, 32),
            Color.FromArgb(17, 176, 60),
            Color.FromArgb(176, 112, 28),
            Color.FromArgb(255, 193, 38),
            Color.FromArgb(102, 102, 102),
            Color.FromArgb(170, 170, 170),
            Color.FromArgb(153, 0, 0),
            Color.FromArgb(255, 0, 19),
            Color.FromArgb(153, 0, 78),
            Color.FromArgb(255, 0, 143),
            Color.FromArgb(0, 80, 205),
            Color.FromArgb(38, 201, 255),
            Color.FromArgb(150, 65, 18),
            Color.FromArgb(255, 120, 41),
            Color.FromArgb(203, 90, 87),
            Color.FromArgb(254, 175, 168)
        };

        public Bitmap PixelImage(Bitmap buf, int resolution, int brightness, string tag)
        {
            Bitmap result = new Bitmap(buf.Width, buf.Height);
            SolidBrush Brush = new SolidBrush(Color.Empty);
            Graphics graph = Graphics.FromImage(result);

            if (buf != null)
            {
                for (int i = 0; i < buf.Width; i += resolution)
                {
                    for (int j = 0; j < buf.Height; j += resolution)
                    {
                        var rect = new Rectangle(i, j, resolution + 1, resolution + 1);
                        Brush.Color = buf.GetPixel(i, j);
                        graph.FillRectangle(ColorChanger(Brush, brightness, tag), rect);
                    }
                }
            }
            return result;
        }

        public SolidBrush ColorChanger(SolidBrush Brush, int brightness, string tag)
        {
            int position = 0, min = 10000, dif = 0;

            SolidBrush bufBrush = new SolidBrush(Color.Empty);
            RGB brushRGB = new RGB(Brush.Color.R, Brush.Color.G, Brush.Color.B);
            XYZ brushXYZ = ColorHelper.ColorConverter.RgbToXyz(brushRGB);

            for (int i = 0; i < GarticColor.Length; i++)
            {
                RGB GarticRGB = new RGB(GarticColor[i].R, GarticColor[i].G, GarticColor[i].B);
                XYZ garticXYZ = ColorHelper.ColorConverter.RgbToXyz(GarticRGB);

                switch(tag)
                {
                    case "RGB":
                        dif = (int)(Math.Pow((GarticColor[i].R - brightness - Brush.Color.R), 2) +
                        Math.Pow((GarticColor[i].G - brightness - Brush.Color.G), 2) +
                        Math.Pow((GarticColor[i].B - brightness - Brush.Color.B), 2));
                        break;

                    case "XYZ":
                        dif = (int)(Math.Pow((garticXYZ.X - brightness - brushXYZ.X), 2) +
                        Math.Pow((garticXYZ.Y - brightness - brushXYZ.Y), 2) +
                        Math.Pow((garticXYZ.Z - brightness - brushXYZ.Z), 2));
                        break;
                }

                if (dif < min)
                {
                    min = dif;
                    position = i;
                }
            }

            bufBrush.Color = GarticColor[position];
            return bufBrush;
        }
    }
}
