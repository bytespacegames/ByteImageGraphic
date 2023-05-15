using System.Diagnostics;
using System.Drawing;

namespace BitmapToCustom
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter File Location:");
            string fileLocation = Console.ReadLine();

            Bitmap bitmap = new Bitmap(fileLocation);

            List<Color> colors = new List<Color>();
            Dictionary<Color, List<Tuple<int, int>>> cols = new();

            //create data of pixels and colors
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    colors.Add(pixelColor);

                    if (!cols.ContainsKey(pixelColor))
                    {
                        cols.Add(pixelColor, new());
                    }

                    cols[pixelColor].Add(Tuple.Create(x, y));
                }
            }

            //sort and get background color
            colors.GroupBy(c => c).OrderByDescending(g => g.Count());
            Color bg = colors[0];

            //count pixels to add to byte size
            int pixels = 0;
            foreach (Color color in cols.Keys)
            {
                if (color != bg)
                {
                    pixels += cols[color].Count;
                }
            }

            byte[] file = new byte[4 + (cols.Count * 4) + (pixels*2)];
            file[0] = bg.R; file[1] = bg.G; file[2] = bg.B; file[3] = (byte)bitmap.Height;

            //index for a byte
            int byteindex = 4;

            foreach (Color color in cols.Keys)
            {
                //dont add data for background
                if (color == bg) { continue;  }

                //add seperator and colors
                file[byteindex] = 255;
                file[byteindex + 1] = color.R; file[byteindex + 2] = color.G; file[byteindex + 2] = color.B;
                byteindex += 2;

                foreach (Tuple<int,int> coordinate in cols[color])
                {
                    file[byteindex] = (byte)coordinate.Item1;
                    file[byteindex + 2] = (byte)coordinate.Item2;

                    byteindex += 2;
                }

            }

            File.WriteAllBytes(Path.GetDirectoryName(fileLocation) + "\\" + Path.GetFileNameWithoutExtension(fileLocation) + ".big", file);
        }
    }
}