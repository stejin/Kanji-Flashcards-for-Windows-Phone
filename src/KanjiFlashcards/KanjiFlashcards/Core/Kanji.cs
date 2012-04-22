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
using KanjiDatabase;
using KanjiFlashcards.ServiceContracts.Types;

namespace KanjiFlashcards.Core
{
    public class Kanji
    {

        public string Literal { get; private set; }

        public string Meaning { get; private set; }

        public string OnYomi { get; private set; }

        public string KunYomi { get; private set; }

        public JLPT JLPTLevel { get; private set; }

        public int StrokeCount { get; private set; }

        public Kanji(KanjiData data) : this(data.Literal, data.Meaning, data.OnYomi, data.KunYomi, (JLPT)data.JLPTLevel, data.StrokeCount)
        {  }

        public Kanji(KanjiMessage kanjiMessage) : this(kanjiMessage.Literal, kanjiMessage.Meaning, kanjiMessage.OnYomi, kanjiMessage.KunYomi, (JLPT)kanjiMessage.Jlpt, kanjiMessage.StrokeCount)
        {  }

        public Kanji(string literal, string meaning, string onYomi, string kunYomi, JLPT jlptLevel, int strokeCount)
        {
            Literal = literal;
            Meaning = meaning;
            OnYomi = onYomi;
            KunYomi = kunYomi;
            JLPTLevel = jlptLevel;
            StrokeCount = strokeCount;
        }

        public override string ToString()
        {
            return Literal.ToString();
        }

    }
}
