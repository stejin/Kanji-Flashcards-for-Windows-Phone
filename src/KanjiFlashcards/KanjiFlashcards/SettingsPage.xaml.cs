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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO;
using KanjiDatabase;

namespace KanjiFlashcards
{
    public partial class Settings : PhoneApplicationPage
    {

        private int latestVersion;
        private bool isLoading;

        public Settings()
        {
            InitializeComponent();

            LoadSettings();

            isLoading = false;
        }

        private void LoadSettings()
        {
            var jlptLevel = App.AppSettings.GetJLPTLevels();
            Jlpt1CheckBox.IsChecked = (jlptLevel & JLPT.Level1) == JLPT.Level1;
            Jlpt2CheckBox.IsChecked = (jlptLevel & JLPT.Level2) == JLPT.Level2;
            Jlpt3CheckBox.IsChecked = (jlptLevel & JLPT.Level3) == JLPT.Level3;
            Jlpt4CheckBox.IsChecked = (jlptLevel & JLPT.Level4) == JLPT.Level4;
            OtherCheckBox.IsChecked = (jlptLevel & JLPT.Other) == JLPT.Other;
            RandomizeFlashcardsCheckBox.IsChecked = App.AppSettings.IsRandomFlashcards;
            RandomizeReviewListCheckBox.IsChecked = App.AppSettings.IsRandomReviewList;
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
        }

        private void SaveSettings()
        {
            JLPT jlptLevels = new JLPT();
            if (Jlpt1CheckBox.IsChecked.Value) jlptLevels |= JLPT.Level1;
            if (Jlpt2CheckBox.IsChecked.Value) jlptLevels |= JLPT.Level2;
            if (Jlpt3CheckBox.IsChecked.Value) jlptLevels |= JLPT.Level3;
            if (Jlpt4CheckBox.IsChecked.Value) jlptLevels |= JLPT.Level4;
            if (OtherCheckBox.IsChecked.Value) jlptLevels |= JLPT.Other;
            App.AppSettings.SetJLPTLevels(jlptLevels);
            App.AppSettings.IsRandomFlashcards = RandomizeFlashcardsCheckBox.IsChecked.Value;
            App.AppSettings.IsRandomReviewList = RandomizeReviewListCheckBox.IsChecked.Value;
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void Jlpt1CheckBox_Click(object sender, RoutedEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void Jlpt2CheckBox_Click(object sender, RoutedEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void Jlpt3CheckBox_Click(object sender, RoutedEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void Jlpt4CheckBox_Click(object sender, RoutedEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void OtherCheckBox_Click(object sender, RoutedEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = App.AppSettings.IsExperimentalFunctionalityEnabled ? "Disable Experimental Features" : "Enable Experimental Features";
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            ((PhoneApplicationFrame)App.Current.RootVisual).Style = null;
            EnterStoryboard.Begin();
            base.OnNavigatedTo(e);
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            if (App.AppSettings.IsExperimentalFunctionalityEnabled) {
                App.AppSettings.IsExperimentalFunctionalityEnabled = false;
                App.AppSettings.DeveloperPassword = String.Empty;
                (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Text = "Enable Experimental Features";
            } else {
                this.NavigationService.Navigate(new Uri("/PasswordPage.xaml", UriKind.Relative));
            }
        }

    }
}