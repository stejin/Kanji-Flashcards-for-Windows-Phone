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
using KanjiFlashcards.Core;
using System.ComponentModel;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Microsoft.Phone.Tasks;

namespace KanjiFlashcards
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool isLoading = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Visibility v = (Visibility)Resources["PhoneLightThemeVisibility"];
            if (v == System.Windows.Visibility.Visible) {
                var icons = new Image[] { PlayIcon, ReviewIcon, LookupIcon, SettingsIcon, AboutIcon, EmailIcon };
                foreach (var icon in icons) {
                    var image = icon.Source as BitmapImage;
                    image.UriSource = new Uri(image.UriSource.OriginalString.Replace(".dark.", ".light."), UriKind.Relative);
                }
            }

            UpdateTodayKanji(App.AppSettings.TodayKanji);

            if (NetworkInterface.GetIsNetworkAvailable()) {

                WebClient client2 = new WebClient();
                client2.OpenReadCompleted += new OpenReadCompletedEventHandler(UpdateTodayKanji);
                client2.OpenReadAsync(new Uri(App.BaseUrl + "/KanjiForToday?" + App.AppSettings.JLPTLevels));

            } 
        }

        private void UpdateTodayKanji(object sender, OpenReadCompletedEventArgs args)
        {
            try {
                var serializer = new XmlSerializer(typeof(KanjiFlashcards.ServiceContracts.Operations.KanjiForTodayResponse));
                KanjiFlashcards.ServiceContracts.Operations.KanjiForTodayResponse response = serializer.Deserialize(args.Result) as KanjiFlashcards.ServiceContracts.Operations.KanjiForTodayResponse;

                App.AppSettings.TodayKanji = response.Kanji;
                UpdateTodayKanji(App.AppSettings.TodayKanji);
            } catch { }
        }

        private void UpdateTodayKanji(KanjiFlashcards.ServiceContracts.Types.KanjiMessage kanji)
        {
            if (kanji.Literal == null || kanji.Literal == String.Empty)
                return;

            //TodayKanjiText.Text = kanji.Literal;
            TodayKanjiImage.Source = new BitmapImage(new Uri(String.Format("/Images/Kanji/{0}.png", kanji.Id), UriKind.Relative));

            System.Text.StringBuilder text = new System.Text.StringBuilder();
            text.Append(TodayKanjiDetails.Tag.ToString());
            text.Append(Environment.NewLine);
            text.Append(kanji.OnYomi);
            text.Append(Environment.NewLine);
            text.Append(kanji.KunYomi);
            text.Append(Environment.NewLine);
            text.Append(kanji.Meaning);

            TodayKanjiDetails.Text = text.ToString();
        }

        private void FlashcardsListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isLoading) {
                e.Handled = true;
                return;
            }
            isLoading = true;

            progressBar.Visibility = System.Windows.Visibility.Visible;

            App.KanjiMode = Mode.Flashcards;

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(LoadKanjiAsync);
            worker.WorkerReportsProgress = false;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadKanjiCompleted);
            worker.RunWorkerAsync();
            e.Handled = true;
        }

        private void LoadKanjiAsync(object sender, DoWorkEventArgs e)
        {
            App.KanjiDict.LoadFromDatabase(App.AppSettings.GetJLPTLevels());
        }

        private void ReviewListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/ReviewListView.xaml?mode=load", UriKind.Relative));
            e.Handled = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            progressBar.Visibility = System.Windows.Visibility.Collapsed;

            if (App.AppSettings.IsExperimentalFunctionalityEnabled) {
                //Enable experimental menu items
                //ContentPanel.RowDefinitions[2].Height = new GridLength(80);
            } else {
                //Disable experimental menu items
                //ContentPanel.RowDefinitions[2].Height = new GridLength(0);
            }

                ((PhoneApplicationFrame)App.Current.RootVisual).Style = (Style)App.Current.Resources["mainFrameStyle"];
            base.OnNavigatedTo(e);
        }

        private void AboutListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            e.Handled = true;
        }

        private void FeatureRequestListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {
                To = "app@stejin.org",
                Subject = "New Feature for Kanji Flashcards App",
                Body = "[Describe requested feature in as much detail as possible.]"
            };
            emailComposeTask.Show();

            e.Handled = true;
        }

        private void LookupListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/LookupView.xaml", UriKind.Relative));
            e.Handled = true;
        }

        private void TodayKanji_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (isLoading) {
                e.Complete();
                e.Handled = true;
                return;
            }
            isLoading = true;

            progressBar.Visibility = System.Windows.Visibility.Visible;

            App.KanjiMode = Mode.Flashcards;

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(LoadTodayKanjiAsync);
            worker.WorkerReportsProgress = false;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadKanjiCompleted);
            worker.RunWorkerAsync();
            e.Complete();
            e.Handled = true;
        }

        private void LoadTodayKanjiAsync(object sender, DoWorkEventArgs e)
        {
            App.KanjiDict.LoadFromDatabase(new List<string>() { App.AppSettings.TodayKanji.Literal });
        }

        private void SettingsListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            e.Handled = true;
        }

        private void LoadKanjiCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isLoading = false;

            progressBar.Visibility = System.Windows.Visibility.Collapsed;

            if (App.KanjiDict.KanjiCount == 0)
                MessageBox.Show("No kanji found.");
            else
                this.NavigationService.Navigate(new Uri("/Views/KanjiView.xaml", UriKind.Relative));
        }

    }
}