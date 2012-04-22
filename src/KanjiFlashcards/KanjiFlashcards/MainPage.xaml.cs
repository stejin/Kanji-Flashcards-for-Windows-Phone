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
using KanjiFlashcards.ServiceContracts.Operations;
using System.Xml.Serialization;
using Microsoft.Phone.Tasks;

namespace KanjiFlashcards
{
    public partial class MainPage : PhoneApplicationPage
    {
        private int latestVersion;
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
                TodayKanji.Background = new SolidColorBrush(new Color() { R = 216, G = 216, B = 216, A = 255});
            }

            UpdateTodayKanji(App.AppSettings.TodayKanji);

            if (NetworkInterface.GetIsNetworkAvailable()) {

                WebClient client1 = new WebClient();
                client1.OpenReadCompleted += new OpenReadCompletedEventHandler(CheckDatabaseVersion);
                client1.OpenReadAsync(new Uri(App.BaseUrl + "/KanjiDatabaseCurrentVersion"));
                WebClient client2 = new WebClient();
                client2.OpenReadCompleted += new OpenReadCompletedEventHandler(UpdateTodayKanji);
                client2.OpenReadAsync(new Uri(App.BaseUrl + "/KanjiForToday?" + App.AppSettings.JLPTLevels));

            } else if (!App.KanjiDict.KanjiDatabaseExists()) {
                MessageBox.Show("Local kanji database not found and no data connection available. Please connect to the internet to download the database.");
            } 
        }

        private void CheckDatabaseVersion(object sender, OpenReadCompletedEventArgs args)
        {
            try {
                var serializer = new XmlSerializer(typeof(KanjiDatabaseCurrentVersionResponse));
                KanjiDatabaseCurrentVersionResponse response = serializer.Deserialize(args.Result) as KanjiDatabaseCurrentVersionResponse;

                latestVersion = response.Version;
                int currentVersion = App.AppSettings.DatabaseVersion;
                if (currentVersion < latestVersion) {
                    MessageBox.Show("Kanji database is out-of-date.  Click OK to download the latest version.");
                    App.KanjiDict.DatabaseUpdateCompleted += DatabaseUpdateCompleted;
                    App.KanjiDict.DatabaseUpdateError += DatabaseUpdateFailed;
                    progressBar.Visibility = System.Windows.Visibility.Visible;
                    isLoading = true;
                    App.KanjiDict.UpdateKanjiDatatabaseFromInternet();
                }
            } catch { }
        }

        private void DatabaseUpdateCompleted(object sender, EventArgs args)
        {
            App.KanjiDict.DatabaseUpdateCompleted -= DatabaseUpdateCompleted;
            App.KanjiDict.DatabaseUpdateError -= DatabaseUpdateFailed;

            App.AppSettings.DatabaseVersion = latestVersion;

            progressBar.Visibility = System.Windows.Visibility.Collapsed;

            MessageBox.Show(String.Format("Database successfully updated to version {0}.", latestVersion));
            isLoading = false;
        }


        private void DatabaseUpdateFailed(object sender, DatabaseUpdateErrorEventArgs args)
        {
            App.KanjiDict.DatabaseUpdateCompleted -= DatabaseUpdateCompleted;
            App.KanjiDict.DatabaseUpdateError -= DatabaseUpdateFailed;

            progressBar.Visibility = System.Windows.Visibility.Collapsed;

            MessageBox.Show(String.Format("Database update failed with error: {0}.", args.ErrorMessage));
            isLoading = false;
        }

        private void UpdateTodayKanji(object sender, OpenReadCompletedEventArgs args)
        {
            try {
                var serializer = new XmlSerializer(typeof(KanjiForTodayResponse));
                KanjiForTodayResponse response = serializer.Deserialize(args.Result) as KanjiForTodayResponse;

                App.AppSettings.TodayKanji = response.Kanji;
                UpdateTodayKanji(App.AppSettings.TodayKanji);
            } catch { }
        }

        private void UpdateTodayKanji(KanjiFlashcards.ServiceContracts.Types.KanjiMessage kanji)
        {
            if (kanji.Literal == null || kanji.Literal == String.Empty)
                return;

            TodayKanjiText.Text = kanji.Literal;

            System.Text.StringBuilder text = new System.Text.StringBuilder();
            text.Append(TodayKanjiDetails.Tag.ToString());
            text.Append(Environment.NewLine).Append(Environment.NewLine);
            text.Append(kanji.OnYomi);
            text.Append(Environment.NewLine);
            text.Append(kanji.KunYomi);
            text.Append(Environment.NewLine);
            text.Append(kanji.Meaning);

            TodayKanjiDetails.Text = text.ToString();
        }

        private void Settings_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
            e.Complete();
            e.Handled = true;
        }

        private void Kanji_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
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
            worker.DoWork += new DoWorkEventHandler(LoadKanjiAsync);
            worker.WorkerReportsProgress = false;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadKanjiCompleted);
            worker.RunWorkerAsync();
            e.Complete();
            e.Handled = true;
        }

        private void LoadKanjiAsync(object sender, DoWorkEventArgs e)
        {
            App.KanjiDict.LoadFromDatabase(App.AppSettings.GetJLPTLevels());
        }

        private void LoadKanjiCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            isLoading = false;

            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            
            if (App.KanjiDict.KanjiCount == 0)
                MessageBox.Show("No kanji found.");
            else
                this.NavigationService.Navigate(new Uri("/KanjiPage.xaml", UriKind.Relative));
        }

        private void ReviewList_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (isLoading) {
                e.Complete();
                e.Handled = true;
                return;
            }
            isLoading = true;

            progressBar.Visibility = System.Windows.Visibility.Visible;

            App.KanjiMode = Mode.Review;

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(LoadReviewListAsync);
            worker.WorkerReportsProgress = false;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadKanjiCompleted);
            worker.RunWorkerAsync();
            e.Complete();
            e.Handled = true;
        }

        private void LoadReviewListAsync(object sender, DoWorkEventArgs e)
        {
            App.KanjiDict.LoadFromDatabase(App.ReviewList.KanjiList);
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

        private void About_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
            e.Complete();
            e.Handled = true;
        }

        private void FeatureRequest_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {
                To = "app@stejin.org",
                Subject = "New Feature for Kanji Flashcards App",
                Body = "[Describe requested feature in as much detail as possible.]"
            };
            emailComposeTask.Show();

            e.Handled = true;
            e.Complete();
        }

        private void Lookup_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/LookupPage.xaml", UriKind.Relative));
            e.Complete();
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

    }
}