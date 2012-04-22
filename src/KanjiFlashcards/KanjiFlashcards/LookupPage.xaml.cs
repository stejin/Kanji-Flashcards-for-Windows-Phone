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
            int kanjiId;
            if (Int32.TryParse(KanjiId.Text, out kanjiId)) {
                if (NetworkInterface.GetIsNetworkAvailable()) {
                    progressBar.Visibility = System.Windows.Visibility.Visible;

                    WebClient client = new WebClient();
                    client.OpenReadCompleted += new OpenReadCompletedEventHandler(LoadKanjiDetails);
                    client.OpenReadAsync(new Uri(App.BaseUrl + "/KanjiForId?" + kanjiId));

                    KanjiId.IsEnabled = false;
                } else {
                    KanjiDetails.Children.Clear();
                    MessageBox.Show("Unable no connect to Kanji Lookup Service.", "No Data Connection", MessageBoxButton.OK);
                }
            } else {
                MessageBox.Show("Please enter a natural number (positive integer) with no leading or trailing spaces.", "Kanji Identifier Not Valid", MessageBoxButton.OK);
            }
        }

        private void LoadKanjiDetails(object sender, OpenReadCompletedEventArgs args)
        {
            KanjiIdentifier.Children.Clear();
            KanjiImage.Children.Clear();
            KanjiDetails.Children.Clear();

            TextBlock kanjiIdentifierText;
            TextBlock kanjiDetailText;

            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            try {
                var serializer = new XmlSerializer(typeof(KanjiForIdResponse));
                KanjiForIdResponse response = serializer.Deserialize(args.Result) as KanjiForIdResponse;

                switch (response.Kanji.Id) {
                    case -1:
                        kanjiDetailText = new TextBlock() { Text = "Kanji Lookup Service temporarily not available. Please try again later.", TextWrapping = TextWrapping.Wrap };
                        KanjiDetails.Children.Add(kanjiDetailText);
                        break;
                    case 0:
                        kanjiDetailText = new TextBlock() { Text = "Kanji not found.", TextWrapping = TextWrapping.Wrap };
                        KanjiDetails.Children.Add(kanjiDetailText);
                        break;
                    default:
                        kanji = new Kanji(response.Kanji);
                        currentKanji = response.Kanji.Id;

                        Image kanjiImage = new Image() { Source = new BitmapImage(new Uri(App.AppSettings.KanjiImageBaseUrl + response.Kanji.Id.ToString() + ".png", UriKind.Absolute)), Width = 110, Height = 110, HorizontalAlignment = System.Windows.HorizontalAlignment.Left };
                        KanjiImage.Children.Add(kanjiImage);

                        kanjiIdentifierText = new TextBlock() { Text = response.Kanji.Literal, FontSize = 96, Margin = new Thickness(0, -20, 0, 0) };
                        KanjiIdentifier.Children.Add(kanjiIdentifierText);

                        System.Text.StringBuilder text = new System.Text.StringBuilder();
                        text.Append("Kun-Yomi: " + response.Kanji.KunYomi);
                        text.Append(Environment.NewLine).Append(Environment.NewLine);
                        text.Append("On-Yomi: " + response.Kanji.OnYomi);
                        text.Append(Environment.NewLine).Append(Environment.NewLine);
                        text.Append("Meaning: " + response.Kanji.Meaning);
                        text.Append(Environment.NewLine).Append(Environment.NewLine);
                        text.Append("JLPT: " + response.Kanji.Jlpt.ToString());
                        text.Append(Environment.NewLine).Append(Environment.NewLine);
                        text.Append("Strokes: " + response.Kanji.StrokeCount.ToString());
                        kanjiDetailText = new TextBlock() { Text = text.ToString(), TextWrapping = TextWrapping.Wrap };
                        KanjiDetails.Children.Add(kanjiDetailText);

                        (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = !App.ReviewList.KanjiList.Contains(kanji.Literal);
                        (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = App.ReviewList.KanjiList.Contains(kanji.Literal);

                        break;
                }
            } catch {
                kanjiDetailText = new TextBlock() { Text = "Kanji Lookup Service temporarily not available. Please try again later.", TextWrapping = TextWrapping.Wrap };
                KanjiDetails.Children.Add(kanjiDetailText);
            }
            KanjiId.IsEnabled = true;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void AddToReviewListButton_Click(object sender, EventArgs e)
        {
            App.ReviewList.KanjiList.Add(kanji.Literal);
            App.ReviewList.Save();
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void RemoveFromReviewListButton_Click(object sender, EventArgs e)
        {
            App.ReviewList.KanjiList.Remove(kanji.Literal);
            App.ReviewList.Save();
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {
                To = "app@stejin.org",
                Subject = "Reported Kanji: " + kanji.Literal,
                Body = "Id: " + currentKanji
                + Environment.NewLine
                + "Literal: " + kanji.Literal
                + Environment.NewLine
                + "On-Yomi: " + kanji.OnYomi
                + Environment.NewLine
                + "Kun-Yomi: " + kanji.KunYomi
                + Environment.NewLine
                + "Meaning: " + kanji.Meaning
                + Environment.NewLine
                + "JLPT: " + kanji.JLPTLevel.ToString()
                + Environment.NewLine
                + "Stroke Count: " + kanji.StrokeCount
                + Environment.NewLine
                + Environment.NewLine
                + "JLPT Setting: " + App.AppSettings.JLPTLevels
            };
            emailComposeTask.Show();
        }
    }
}