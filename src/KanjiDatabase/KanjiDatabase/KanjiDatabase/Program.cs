using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NeoDatis.Odb;

namespace KanjiDatabase
{
    class Program
    {
        static void Main(string[] args)
        {

           // CopyToDatabase();
            //   GenerateDatabase("kanjidic2.neodatis");
            // TestDatabase("kanjidic2.neodatis");
            TestDatabase2();
        }

        private static void GenerateDatabase(string databaseName)
        {
            var kanjiList = LoadFromFile();

            ODB odb = ODBFactory.Open(databaseName);

            kanjiList.ForEach(k => odb.Store(k));

       //     odb.GetClassRepresentation(typeof(KanjiData)).AddUniqueIndexOn("literal-index", new string[] { "Literal" }, true);

       //     odb.GetClassRepresentation(typeof(KanjiData)).AddIndexOn("jlpt-index", new string[] { "JLPTLevel" }, true);

            odb.Commit();

            odb.Close();
        }

        private static void TestDatabase(string databaseName)
        {
            ODB odb = ODBFactory.Open(databaseName);

            var data = odb.GetObjects<KanjiData>();           

            odb.Close();
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
            kanjiTable.Values.ToList<KanjiData>().ForEach(k => kanjiInDatabase.Add(k.Literal));
            ODB odb = ODBFactory.Open(@"C:\Users\Steffen\Documents\Projects\KanjiDatabase\kanjidic2.neodatis");
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

    }
}
