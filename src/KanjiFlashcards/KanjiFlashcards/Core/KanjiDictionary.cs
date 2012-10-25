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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using KanjiDatabase;

namespace KanjiFlashcards.Core
{
    public class KanjiDictionary
    {
        private Dictionary<string, Kanji> kanjiDictionary;

        private List<int> sequence;

        private const string connectionString = @"Data Source = 'appdata:/Dictionary/KanjiDB.sdf'; File Mode = read only;";

        public int KanjiCount
        {
            get
            {
                if (kanjiDictionary != null)
                    return kanjiDictionary.Count;
                return 0;
            }
        }

        public void LoadFromDatabase(JLPT jlptLevels)
        {
            kanjiDictionary = new Dictionary<string, Kanji>();

            using (KanjiDataContext context = new KanjiDataContext(connectionString)) {

                var data = from k in context.Kanji where k.JLPTLevel == (jlptLevels & k.JLPTLevel) select k;

                data.ToList().ForEach(k => kanjiDictionary.Add(k.Literal, k));

                if (App.AppSettings.IsRandomFlashcards)
                    GenerateRandomSequence();
                else
                    GenerateSequence();
            }
                
        }

        public void LoadFromDatabase(List<string> kanjiList)
        {
            kanjiDictionary = new Dictionary<string, Kanji>();

            using (KanjiDataContext context = new KanjiDataContext(connectionString)) {

                var data = from k in context.Kanji where kanjiList.Contains(k.Literal) select k;

                kanjiList.ForEach(k => kanjiDictionary.Add(k, data.First(d => d.Literal == k)));

                if (App.AppSettings.IsRandomReviewList)
                    GenerateRandomSequence();
                else
                    GenerateSequence();
            }
          
        }

        public void SetDictionary(Dictionary<string, Kanji> dictionary)
        {
            kanjiDictionary = dictionary;
            GenerateSequence();
        }

        public Kanji GetKanjiFromDatabase(int kanjiId)
        {
            using (KanjiDataContext context = new KanjiDataContext(connectionString)) {

                var data = from k in context.Kanji where k.Id == kanjiId select k;

                if (data.Count() == 0)
                    return null;

                return data.First();
            }
        }

        public Kanji GetKanjiFromDatabase(string literal)
        {
            using (KanjiDataContext context = new KanjiDataContext(connectionString)) {

                var data = from k in context.Kanji where k.Literal == literal select k;

                if (data.Count() == 0)
                    return null;

                return data.First();
            }
        }

        private void GenerateSequence()
        {
            sequence = new List<int>(kanjiDictionary.Count);

            List<int> temp = new List<int>(kanjiDictionary.Count);

            for (int i = 0; i < kanjiDictionary.Count; i++)
                sequence.Add(i);
        }

        private void GenerateRandomSequence()
        {
            sequence = new List<int>(kanjiDictionary.Count);

            List<int> temp = new List<int>(kanjiDictionary.Count);

            for (int i = 0; i < kanjiDictionary.Count; i++)
                temp.Add(i);

            Random rand = new Random(DateTime.Now.Millisecond);

            while (sequence.Count < kanjiDictionary.Count) {
                int index = rand.Next(0, temp.Count);
                sequence.Add(temp[index]);
                temp.RemoveAt(index);
            }
        }

        public Kanji GetKanji(int index)
        {
            return kanjiDictionary.Values.ElementAt<Kanji>(sequence[index]);
        }

        public bool IsIndexValid(int index)
        {
            return (index > -1 && index < kanjiDictionary.Count);
        }

        public int GetKanjiIndex(string kanji)
        {
            if (kanjiDictionary.ContainsKey(kanji))
                return sequence.IndexOf(kanjiDictionary.Keys.ToList().IndexOf(kanji));
            else
                return 0;
        }

        public void RemoveKanjiFromDictionary(string literal)
        {
            kanjiDictionary.Remove(literal);
        }

    }
}
