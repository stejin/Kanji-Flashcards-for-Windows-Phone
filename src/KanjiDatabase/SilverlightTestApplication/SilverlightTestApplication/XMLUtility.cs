using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using KanjiDatabase;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace SilverlightTestApplication
{
    public class XMLUtility
    {
        private Dictionary<int, XElement> kanjiDictionary;
        
        public void LoadFromFile()
        {
            XDocument dict = XDocument.Load("Dictionary/kanjidic2.xml");

            var kanjiList = from character in dict.Descendants("character") 
                            select character;

            kanjiDictionary = new Dictionary<int, XElement>();

            int i = 0;

            foreach (var kanji in kanjiList) {
                kanjiDictionary.Add(i, kanji);
                i++;
            }
        }

        public List<Kanji> GetAllKanji()
        {
            var result = new List<Kanji>(kanjiDictionary.Count);
            foreach (var x in kanjiDictionary.Values.ToList<XElement>()) {
                Kanji kanji = ParseElement(x);
                if (kanji != null)
                    result.Add(kanji);
            }
            return result;
        }

        private JLPT GetJLPTLevel(XElement kanji)
        {
            var jlptElement = kanji.Element("misc").Element("jlpt");
            if (jlptElement == null) {
                return JLPT.Other;
            } else {
                return (JLPT)(int)Math.Pow(2, (double)kanji.Element("misc").Element("jlpt"));
            }
        }

        private Kanji ParseElement(XElement kanji){
            try {
                string literal = (string)kanji.Element("literal");

                StringBuilder meaning = new StringBuilder();
                kanji.Element("reading_meaning").Element("rmgroup").Elements("meaning").Where(m => m.HasAttributes == false).ToList().ForEach(m => meaning.Append(m.Value).Append(", "));

                StringBuilder onYomi = new StringBuilder();
                kanji.Element("reading_meaning").Element("rmgroup").Elements("reading").Where(m => m.Attribute("r_type").Value == "ja_on").ToList().ForEach(m => onYomi.Append(m.Value).Append(", "));

                StringBuilder kunYomi = new StringBuilder();
                kanji.Element("reading_meaning").Element("rmgroup").Elements("reading").Where(m => m.Attribute("r_type").Value == "ja_kun").ToList().ForEach(m => kunYomi.Append(m.Value).Append(", "));

                JLPT jlpt = GetJLPTLevel(kanji);

                int strokeCount = (int)kanji.Element("misc").Element("stroke_count");

                return new Kanji(literal, meaning.ToString().TrimEnd(',', ' '), onYomi.ToString().TrimEnd(',', ' '), kunYomi.ToString().TrimEnd(',', ' '), jlpt, strokeCount);
            } catch {
                return null;
            }
        }

        public Kanji GetKanji(int index)
        {
            if (kanjiDictionary.ContainsKey(index))
                return ParseElement(kanjiDictionary[index]);
            else
                return null; 
        }

        public bool IsIndexValid(int index)
        {
            return kanjiDictionary.ContainsKey(index);
        }

    }
}
