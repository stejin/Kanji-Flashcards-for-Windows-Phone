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

namespace SilverlightTestApplication
{
    public class Kanji
    {

        public string Literal { get; private set; }

        public string Meaning { get; private set; }

        public string OnYomi { get; private set; }

        public string KunYomi { get; private set; }

        public JLPT JLPTLevel { get; private set; }

        public int StrokeCount { get; private set; }

        public Kanji(string literal, string meaning, string onYomi, string kunYomi, JLPT jLptLevel, int strokeCount)
        {
            Literal = literal;
            Meaning = meaning;
            OnYomi = onYomi;
            KunYomi = kunYomi;
            JLPTLevel = jLptLevel;
            StrokeCount = strokeCount;
        }

        public override string ToString()
        {
            return Literal.ToString();
        }

    }
}
