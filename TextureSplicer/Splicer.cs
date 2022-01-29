using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace TextureSplicer
{
    public class Splicer
    {
        Bitmap spriteSheet = null;
        List<Lazy<Bitmap>?> bitmaps;

        public Bitmap SpliceTextures(string[] filePaths)
        {
            List<Task> spliceTasks = new List<Task>();
#pragma warning disable CA1416
#pragma warning disable CS8619
            bitmaps = LoadLazyBitmaps(filePaths);
#pragma warning restore CS8619

            Vector2 heightAndWidth = new Vector2(bitmaps[0].Value.Width, bitmaps[0].Value.Height);

            int sqrt = (int)Math.Ceiling(Math.Sqrt(bitmaps.Count));
            spriteSheet = new Bitmap((int)(sqrt * heightAndWidth.X), (int)(sqrt * heightAndWidth.Y));
            Console.WriteLine("Atlas size {0}x{1}", spriteSheet.Height, spriteSheet.Width);

            Vector2 offset = new Vector2(0, spriteSheet.Height - heightAndWidth.Y);
            for (int i = 0; i < bitmaps.Count; i++)
            {
                Lazy<Bitmap>? lazyBitmap = bitmaps[i];
                
                if (lazyBitmap == null)
                    continue;

                if (offset.X >= spriteSheet.Width)
                {
                    offset.Y = Math.Clamp(offset.Y - heightAndWidth.Y, 0, spriteSheet.Height - heightAndWidth.Y);
                    offset.X = 0;
                }

                Bitmap bitmap = lazyBitmap.Value;

                spliceTasks.Add(SpliceTextureAsync(bitmap, offset, i));

                offset.X += heightAndWidth.X;
            }

            Task.WaitAll(spliceTasks.ToArray());
            return spriteSheet;
        }

        private async Task SpliceTextureAsync(Bitmap bitmap, Vector2 offset, int index)
        {
            await Task.Run(() => 
            {  
                Dictionary<Vector2, Color> pixels = GetAllPixels(bitmap);

                foreach (KeyValuePair<Vector2, Color> pixel in pixels)
                {
                    lock (spriteSheet)
                    {
                        spriteSheet.SetPixel((int)(pixel.Key.X + offset.X), (int)(pixel.Key.Y + offset.Y), pixel.Value);
                    }
                }
                Console.WriteLine("Bitmap {0}/{1}", index, bitmaps.Count);
            });
        }

        private static Dictionary<Vector2, Color> GetAllPixels(Bitmap bitmap)
        {
            Dictionary<Vector2, Color> pixels = new();
            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                    pixels.Add(new Vector2(x, y), bitmap.GetPixel(x, y));

            return pixels;
        }

        private static List<Lazy<Bitmap>> LoadLazyBitmaps(string[] filePaths)
        {
            List<Lazy<Bitmap>> bitmaps = new();

            foreach (string i in filePaths)
                bitmaps.Add(new Lazy<Bitmap>(new Bitmap(i)));
            return bitmaps;
        }

        private static Vector2 GetTotalHeightAndWidth(List<Lazy<Bitmap>?> bitmaps)
        {
            int totalWidth = 0;
            int totalHeight = 0;

            bitmaps.ForEach(bmp =>
            {
                if (bmp != null)
                {
                    Bitmap bitmap = bmp.Value;

                    totalWidth += bitmap.Width;
                    totalHeight += bitmap.Height;

                    bmp = null;
                }
            });

            return new Vector2(totalWidth, totalHeight);
        }
    }
#pragma warning restore CA1416
}