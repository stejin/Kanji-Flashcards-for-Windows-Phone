using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NeoDatis.Odb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;

namespace KanjiDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start.");
            Console.ReadKey();

            //  CopyToDatabase();
            GenerateDatabase(@"C:\Users\Steffen\Documents\GitHub\Kanji-Flashcards-for-Windows-Phone\src\KanjiDatabase\kanjidic2.neodatis");
            TestDatabase(@"C:\Users\Steffen\Documents\GitHub\Kanji-Flashcards-for-Windows-Phone\src\KanjiDatabase\kanjidic2.neodatis");
     //       TestDatabase2();
        }

        private static void GenerateDatabase(string databaseName)
        {
            Console.WriteLine("Generate " + databaseName);
            // var kanjiList = LoadFromFile();
            var kanjiList = DataGateway.Instance.GetAllKanji();
            Console.WriteLine("{0} kanji read", kanjiList.Count);

            using (ODB odb = ODBFactory.Open(databaseName))  {

                foreach (var kanji in kanjiList) {
                    kanji.Image = GetImage(kanji.Literal);
                    odb.Store(kanji);
                }

                Console.WriteLine("Objects saved.");
                
                odb.GetClassRepresentation(typeof(KanjiData)).AddUniqueIndexOn("id-index", new string[] { "Id" }, true);
                
                odb.GetClassRepresentation(typeof(KanjiData)).AddUniqueIndexOn("literal-index", new string[] { "Literal" }, true);

                odb.GetClassRepresentation(typeof(KanjiData)).AddIndexOn("jlpt-index", new string[] { "JLPTLevel" }, true);

                Console.WriteLine("Indexes created.");

                odb.Commit();
            }

            Console.WriteLine("Changes committed. Database closed.");
        }

        private static void TestDatabase(string databaseName)
        {
            using (ODB odb = ODBFactory.Open(databaseName)) {
                var data = odb.GetObjects<KanjiData>();
                Console.WriteLine("{0} objects in database {1}", data.Count, databaseName);
            }
        }

        private static List<KanjiData> LoadFromFile()
        {
            XDocument dict = XDocument.Load(@"C:\Users\Steffen\Documents\Projects\KanjiDatabase\kanjidic2.xml");

            var kanjiList = from character in dict.Descendants("character") 
                            select character;

            List<KanjiData> result = new List<KanjiData>(kanjiList.Count());

            foreach (var kanji in kanjiList) {
                if (HasMeaning(kanji))
                    result.Add(ParseElement(kanji));
            }

            return result;
        }

        private static KanjiData ParseElement(XElement kanji)
        {
            string literal = (string)kanji.Element("literal");

            StringBuilder meaning = new StringBuilder();
            kanji.Element("reading_meaning").Element("rmgroup").Elements("meaning").Where(m => m.HasAttributes == false).ToList().ForEach(m => meaning.Append(m.Value).Append(", "));

            StringBuilder onYomi = new StringBuilder();
            kanji.Element("reading_meaning").Element("rmgroup").Elements("reading").Where(m => m.Attribute("r_type").Value == "ja_on").ToList().ForEach(m => onYomi.Append(m.Value).Append(", "));

            StringBuilder kunYomi = new StringBuilder();
            kanji.Element("reading_meaning").Element("rmgroup").Elements("reading").Where(m => m.Attribute("r_type").Value == "ja_kun").ToList().ForEach(m => kunYomi.Append(m.Value).Append(", "));

            JLPT jlpt = GetJLPTLevel(kanji);

            int strokeCount = (int)kanji.Element("misc").Element("stroke_count");

            return new KanjiData() { Literal = literal, Meaning = meaning.ToString().TrimEnd(',', ' '), OnYomi = onYomi.ToString().TrimEnd(',', ' '), KunYomi = kunYomi.ToString().TrimEnd(',', ' '), JLPTLevel = jlpt, StrokeCount = strokeCount };
        }

        private static bool HasMeaning(XElement kanji)
        {
            var meaning = kanji.Element("reading_meaning");
            return meaning != null;
        }

        private static JLPT GetJLPTLevel(XElement kanji)
        {
            var jlptElement = kanji.Element("misc").Element("jlpt");
            if (jlptElement == null) {
                return JLPT.Other;
            } else {
                return (JLPT)(int)Math.Pow(2, (double)kanji.Element("misc").Element("jlpt"));
            }
        }

        private static void CopyToDatabase()
        {
            Console.WriteLine("Press any key to start");
            Console.ReadKey();
            var kanjiTable = DataGateway.Instance.GetAllKanji();
            List<string> kanjiInDatabase = new List<string>(kanjiTable.Count);
            kanjiTable.ForEach(k => kanjiInDatabase.Add(k.Literal));
            ODB odb = ODBFactory.Open(@"C:\Users\Steffen\Documents\GitHub\Kanji-Flashcards-for-Windows-Phone\src\KanjiDatabase\kanjidic2.neodatis");
            var kanjiList = odb.GetObjects<KanjiData>();
            foreach (var kanji in kanjiList) {
                if (kanjiInDatabase.Contains(kanji.Literal)) {
                    DataGateway.Instance.UpdateKanji(kanji);
                    Console.WriteLine("Kanji {0} updated.", kanji.Literal);
                } else {
                    var response = DataGateway.Instance.InsertKanji(kanji);
                    Console.WriteLine("Kanji {0} created with key {1}.", kanji.Literal, response.SavedObjectKey.ToString());
                }
            }
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void TestDatabase2()
        {
            var list = DataGateway.Instance.GetAllKanji();
        }

        private static MemoryStream GetImage(string kanji)
        {
            Bitmap bmpImage = new Bitmap(1, 1);

            int width = 173;
            int height = 173;
            // Create the Font object for the image text drawing.   
            Font font = new Font("Calibri", 67, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);

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
            graphics.Clear(Color.FromArgb(70, 70, 70));
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            graphics.DrawString(kanji, font, new SolidBrush(Color.White), 39, 47);
            graphics.Flush();

            MemoryStream stream = new MemoryStream();

            bmpImage.Save(stream, ImageFormat.Bmp);
            stream.Close();
            return stream;

           // return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmpImage.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(width, height));
        }

    }
}
