using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KanjiDatabase;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Drawing.Imaging;

namespace GenerateKanjiImages
{
    class Program
    {
        static void Main(string[] args)
        {
            GenerateImages();            
        }

        private static void GenerateImages()
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            var kanjiList = DataGateway.Instance.GetAllKanji();
            foreach (var kanji in kanjiList) {
                var kanjiImage = CreateBitmapImage(kanji.Literal);
                kanjiImage.Save(String.Format(@"C:\Users\Steffen\Documents\GitHub\Kanji-Flashcards-for-Windows-Phone\src\KanjiImages\{0}.png", kanji.Id.ToString()), ImageFormat.Png);
                Console.WriteLine("{0}.png generated.", kanji.Id.ToString());
                //if (kanji.Id == 100)
                //    return;
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static Bitmap CreateBitmapImage(string kanji)
        {
            Bitmap bmpImage = new Bitmap(1, 1);

            // Online
            //int width = 173;
            //int height = 173;
            //int x = 39;
            //int y = 47;
            //int fontSize = 67;

            // Phone
            int width = 173;
            int height = 173;
            int x = 0;
            int y = 21;
            int fontSize = 120;

            // Create the Font object for the image text drawing.   
            Font font = new Font("Calibri", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);

            // Create a graphics object to measure the text's width and height.  
            Graphics graphics = Graphics.FromImage(bmpImage);

            // This is where the bitmap size is determined. 
           // width = (int)objGraphics.MeasureString(kanji, font).Width;
            //height = (int)objGraphics.MeasureString(kanji, font).Height;
            // Create the bmpImage again with the correct size for the text and font. 
            bmpImage = new Bitmap(bmpImage, new Size(width, height));

            // Add the colors to the new bitmap.  
            graphics = Graphics.FromImage(bmpImage);

            // Set Background color  
            graphics.Clear(Color.FromArgb(0, 70, 70, 70));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(kanji, font, new SolidBrush(Color.White), x, y);
            graphics.Flush();

            return (bmpImage);
        }
      
    }
}
