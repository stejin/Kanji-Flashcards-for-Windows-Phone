using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Net.NetworkInformation;
using KanjiFlashcards.ServiceContracts.Operations;
using System.Xml.Serialization;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using KanjiFlashcards.Core;
using System.Text;

namespace KanjiFlashcards
{
    public partial class LookupPage : PhoneApplicationPage
    {
        private int currentKanji;

        private KanjiFlashcards.Core.Kanji kanji;

        public LookupPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = System.Windows.Visibility.Visible;
            RequestDetails.Children.Clear();
            var result = new Dictionary<string, Kanji>();
            foreach (string token in GetTokens(KanjiInput.Text)) {
                TextBlock currentTextBlock = new TextBlock();
                currentTextBlock.FontFamily = new FontFamily("Yu Gothic");
                currentTextBlock.Text = String.Format("{0} => ", token);
                RequestDetails.Children.Add(currentTextBlock);
                Kanji kanji;
                int kanjiId;
                if (Int32.TryParse(token, out kanjiId)) {
                    kanji = App.KanjiDict.GetKanjiFromDatabase(kanjiId);
                } else {
                    kanji = App.KanjiDict.GetKanjiFromDatabase(token);
                }
                if (kanji == null ) {
                    currentTextBlock.Text += "Not found";
                    continue;
                }
                currentTextBlock.Text += kanji.Literal + " : ";
                if (result.ContainsKey(kanji.Literal)) {
                    currentTextBlock.Text += "Skipped (already added)";
                } else {
                    currentTextBlock.Text += "Added";                                    
                    result.Add(kanji.Literal, kanji);
                }
            }
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            if (result.Count == 0) {
                TextBlock failureTextBlock = new TextBlock();
                failureTextBlock.FontFamily = new FontFamily("Yu Gothic");
                Color currentAccentColor = (Color)Application.Current.Resources["PhoneAccentColor"];
                failureTextBlock.Foreground = new SolidColorBrush(currentAccentColor); 
                failureTextBlock.Text = "No kanji found. Please try again.";
                RequestDetails.Children.Add(failureTextBlock);
            } else {
                App.KanjiDict.SetDictionary(result);
                this.NavigationService.Navigate(new Uri("/KanjiPage.xaml", UriKind.Relative));
            }
        }

        private string Cleanse(string input)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (Char.IsWhiteSpace(c) || Char.IsControl(c) || Char.IsPunctuation(c) || Char.IsSeparator(c) || Char.IsSymbol(c) ) {
                    continue;
                }
                if (i > 0 && Char.IsNumber(c) != Char.IsNumber(input, i - 1)) {
                    result.Append(" ");
                }
                result.Append(c);                
            }
            return result.ToString();
        }

        private List<string> GetTokens(string text) {
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