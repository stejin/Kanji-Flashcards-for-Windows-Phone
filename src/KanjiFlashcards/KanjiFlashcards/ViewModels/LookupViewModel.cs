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
using System.ComponentModel;
using System.Text;
using KanjiFlashcards.Core;
using System.Collections.Generic;
using KanjiDatabase;
using System.Collections.ObjectModel;
using System.Linq;

namespace KanjiFlashcards.ViewModels
{
    public class LookupViewModel : INotifyPropertyChanged
    {

        private string lookupString;
        public string LookupString
        {
            get
            {
                return lookupString;
            }
            set
            {
                if (lookupString != value) {
                    lookupString = value;
                    RaisePropertyChanged("LookupString");
                }
            }
        }

        public ObservableCollection<LookupMessageViewModel> LookupMessages { get; set; }

        private Dictionary<string, Kanji> lookupResult;

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Load()
        {
            StringBuilder text = new StringBuilder();
            if (App.LookupKanji == null)
                App.LookupKanji = new List<string>();
            App.LookupKanji.ForEach(l => text.Append(l));
            LookupString = text.ToString();
            LookupMessages = new ObservableCollection<LookupMessageViewModel>();
            lookupResult = new Dictionary<string, Kanji>();
        }

        public OperationResult Save()
        {
            var result = new OperationResult();
            App.LookupKanji = lookupResult.Keys.ToList();
            App.KanjiDict.SetDictionary(lookupResult);
            if (lookupResult.Count == 0) {
                result.Result = Result.Failure;
                result.Message = "No kanji found. Please try again.";
            } else {
                result.Result = Result.Success;
            }
            return result;
        }

        public void Parse()
        {
            foreach (var token in GetTokens(LookupString)) {

                var lookupMessage = new LookupMessageViewModel();
                lookupMessage.ItemToLookup = token;
                Kanji kanji;
                int kanjiId;
                if (Int32.TryParse(token, out kanjiId)) {
                    kanji = App.KanjiDict.GetKanjiFromDatabase(kanjiId);
                } else {
                    kanji = App.KanjiDict.GetKanjiFromDatabase(token);
                }
                if (kanji == null) {
                    lookupMessage.LookupResult = "Not found";
                }
                lookupMessage.LookupResult += kanji.Literal + " : ";
                if (lookupResult.ContainsKey(kanji.Literal)) {
                    lookupMessage.LookupResult += "Skipped (already added)";
                } else {
                    lookupMessage.LookupResult += "Added";
                    lookupResult.Add(kanji.Literal, kanji);
                }
            }
        }

        private string Cleanse(string input)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (Char.IsWhiteSpace(c) || Char.IsControl(c) || Char.IsPunctuation(c) || Char.IsSeparator(c) || Char.IsSymbol(c)) {
                    continue;
                }
                if (i > 0 && Char.IsNumber(c) != Char.IsNumber(input, i - 1)) {
                    result.Append(" ");
                }
                result.Append(c);
            }
            return result.ToString();
        }

        private List<string> GetTokens(string text)
        {
            var result = new List<string>();
            var kanjiText = Cleanse(text);
            foreach (string token in kanjiText.Split(' ')) {
                int kanjiId;
                if (Int32.TryParse(token, out kanjiId)) {
                    result.Add(kanjiId.ToString());
                } else {
                    foreach (char literal in token) {
                        result.Add(literal.ToString());
                    }
                }
            }
            return result;
        }

    }
}
