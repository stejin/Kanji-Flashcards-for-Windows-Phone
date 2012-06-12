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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace KanjiFlashcards
{
    public partial class KanjiPage : PhoneApplicationPage
    {
        private int currentKanji;

        private KanjiFlashcards.Core.Kanji kanji;

        public KanjiPage()
        {
            InitializeComponent();

            ExitToLeft.Completed += new EventHandler(ExitToLeft_Completed);
            ExitToRight.Completed += new EventHandler(ExitToRight_Completed);

        }

        private void ExitToLeft_Completed(object sender, EventArgs args)
        {
            ShowKanji();
            EnterFromRight.Begin();
        }

        private void ExitToRight_Completed(object sender, EventArgs args)
        {
            ShowKanji();
            EnterFromLeft.Begin();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.KanjiMode == Core.Mode.Review && ApplicationBar.Buttons.Count == 2)
                ApplicationBar.Buttons.Remove(ApplicationBar.Buttons[0]);

            // (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = App.AppSettings.IsExperimentalFunctionalityEnabled;

            if (App.IsResumeKanji)
                currentKanji = App.KanjiDict.GetKanjiIndex(App.CurrentKanji);
            else
                currentKanji = 0;

            if (App.KanjiDict.IsIndexValid(currentKanji)) {
                ShowKanji();
            } else {
                this.NavigationService.GoBack();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.CurrentKanji = String.Empty;
            base.OnBackKeyPress(e);
        }

        private void ShowKanji()
        {
            DetailsBlock.Text = "";
            kanji = App.KanjiDict.GetKanji(currentKanji);
            App.CurrentKanji = kanji.Literal;
            Kanji.Text = kanji.Literal;
            switch (App.KanjiMode) {
                case Core.Mode.Flashcards:
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = !App.ReviewList.KanjiList.Contains(kanji.Literal);
                    (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = App.ReviewList.KanjiList.Contains(kanji.Literal);
                    break;
                case Core.Mode.Review:
                    (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = App.ReviewList.KanjiList.Contains(kanji.Literal);
                    break;
            }
        }
        
        private void ShowDetails()
        {
            if (DetailsBlock.Text != string.Empty)
                return;

             System.Text.StringBuilder text = new System.Text.StringBuilder();
             for (int i = 0; i < 5; i++) {
                 if (i > 0)
                     text.Append(Environment.NewLine).Append(Environment.NewLine);
                 switch (i) {
                     case 0:
                         text.Append("On-Yomi" + Environment.NewLine + kanji.OnYomi);
                         break;
                     case 1:
                         text.Append("Kun-Yomi" + Environment.NewLine + kanji.KunYomi);
                         break;
                     case 2:
                         text.Append("Meaning" + Environment.NewLine + kanji.Meaning);
                         break;
                     case 3:
                         text.Append("JLPT Level: " + kanji.JLPTLevel.ToString().Replace("Level", ""));
                         break;
                     case 4:
                         text.Append("Strokes: " + kanji.StrokeCount.ToString());
                         break;
                 }
             }
             DetailsBlock.Text = text.ToString();           
        }

        private void MoveForward()
        {
            ((PhoneApplicationFrame)App.Current.RootVisual).Style = (Style)App.Current.Resources["forwardTransition"];

            if (!App.KanjiDict.IsIndexValid(currentKanji + 1)) {
                MessageBox.Show("Reached last kanji in list.");
                return;
            }

            currentKanji += 1;

            ExitToLeft.Begin();
        }

        private void MoveBackward()
        {
            ((PhoneApplicationFrame)App.Current.RootVisual).Style = (Style)App.Current.Resources["backwardTransition"];

            if (!App.KanjiDict.IsIndexValid(currentKanji - 1)) {
                MessageBox.Show("Reached first kanji in list.");
                return;
            }

            currentKanji -= 1;

            ExitToRight.Begin();
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs args)
        {
            if (args.TotalManipulation.Translation.X == 0)
                ShowDetails();

            if (args.TotalManipulation.Translation.X > 0)
                MoveBackward();

            if (args.TotalManipulation.Translation.X < 0)
                MoveForward();

            args.Handled = true;
            base.OnManipulationCompleted(args);
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
            if (App.KanjiMode == Core.Mode.Flashcards) {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
                (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = false;
            } else {
                (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            }
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