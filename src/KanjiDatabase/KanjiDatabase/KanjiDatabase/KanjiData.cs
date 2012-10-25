using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace KanjiDatabase
{
    public class KanjiData
    {

        public int Id;

        public string Literal;

        public string Meaning;

        public string OnYomi;

        public string KunYomi;

        public JLPT JLPTLevel;

        public int StrokeCount;

        public MemoryStream Image;

    }
}
