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
using KanjiFlashcards.ViewModels;

namespace KanjiFlashcards.Views
{
    public partial class KanjiView : PhoneApplicationPage
    {
        private KanjiViewModel vm;

        public KanjiView()
        {
            InitializeComponent();
            vm = new KanjiViewModel();
            
            ExitToLeft.Completed += new EventHandler(ExitToLeft_Completed);
            ExitToRight.Completed += new EventHandler(ExitToRight_Completed);

            vm.OnMoveForward += MoveForward;
            vm.OnMoveForward += UpdateBindings;
            vm.OnMoveBackward += MoveBackward;
            vm.OnMoveBackward += UpdateBindings;
            vm.OnAddItemToReviewList += AddToReviewListButton_Click;
            vm.OnRemoveItemFromReviewList += RemoveFromReviewListButton_Click;
            vm.OnRemoveItemFromReviewList += UpdateBindings;
            vm.OnReportBrokenKanji += ApplicationBarMenuItem_Click;
        }

        private void ExitToLeft_Completed(object sender, EventArgs args)
        {
            vm.NotifyPropertyChanges();
            EnterFromRight.Begin();
        }

        private void ExitToRight_Completed(object sender, EventArgs args)
        {
            vm.NotifyPropertyChanges();
            EnterFromLeft.Begin();
        }

        // Work around bugs in BindableApplicationBar
        private void UpdateBindings(object sender, EventArgs e)
        {
            (ApplicationBar.Buttons[0] as IApplicationBarIconButton).IsEnabled = vm.HasPrevious;
            (ApplicationBar.Buttons[1] as IApplicationBarIconButton).IsEnabled = vm.HasPrevious;
            (ApplicationBar.Buttons[2] as IApplicationBarIconButton).IsEnabled = vm.HasNext;
            (ApplicationBar.Buttons[3] as IApplicationBarIconButton).IsEnabled = vm.HasNext;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if ((App.KanjiMode == Core.Mode.Flashcards || App.KanjiMode == Core.Mode.Lookup) && ApplicationBar.MenuItems.Count == 3)
                ApplicationBar.MenuItems.Remove(ApplicationBar.MenuItems[1]);

            if (App.KanjiMode == Core.Mode.Review && ApplicationBar.MenuItems.Count == 3)
                ApplicationBar.MenuItems.Remove(ApplicationBar.MenuItems[0]);

            // (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = App.AppSettings.IsExperimentalFunctionalityEnabled;

            if (App.IsResumeKanji)
                vm.Index = App.KanjiDict.GetKanjiIndex(App.CurrentKanji.Literal);
            else
                vm.Index = 0;
            App.IsResumeKanji = false;

            this.DataContext = vm;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            App.ReviewList.Save();
            App.CurrentKanji = null;
            base.OnBackKeyPress(e);
        }

        private void MoveForward(object sender, EventArgs e)
        {
            ((PhoneApplicationFrame)App.Current.RootVisual).Style = (Style)App.Current.Resources["forwardTransition"];
            if (ExitToLeft.GetCurrentState() == ClockState.Active)
                vm.NotifyPropertyChanges();
            ExitToLeft.Begin();
        }

        private void MoveBackward(object sender, EventArgs e)
        {
            ((PhoneApplicationFrame)App.Current.RootVisual).Style = (Style)App.Current.Resources["backwardTransition"];
            if (ExitToRight.GetCurrentState() == ClockState.Active)
                vm.NotifyPropertyChanges();
            ExitToRight.Begin();
        }

       
        //protected override void OnManipulationCompleted(ManipulationCompletedEventArgs args)
        //{
        //    if (args.TotalManipulation.Translation.X > 0)
        //        MoveBackward();

        //    if (args.TotalManipulation.Translation.X < 0)
        //        MoveForward();

        //    args.Handled = true;
        //    base.OnManipulationCompleted(args);
        //}

        private void AddToReviewListButton_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/ReviewListView.xaml?mode=add", UriKind.Relative));
        }

        private void RemoveFromReviewListButton_Click(object sender, EventArgs e)
        {
            vm.RemoveItemFromReviewList();
            if (App.CurrentKanji != null) {
                vm.NotifyPropertyChanges();
            } else {
                this.NavigationService.GoBack();
            }
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {
                To = "app@stejin.org",
                Subject = "Reported Kanji: " + App.CurrentKanji.Literal,
                Body = "Id: " + App.CurrentKanji.Id
                + Environment.NewLine
                + "Literal: " + App.CurrentKanji.Literal 
                + Environment.NewLine
                + "On-Yomi: " + App.CurrentKanji.OnYomi 
                + Environment.NewLine
                + "Kun-Yomi: " + App.CurrentKanji.KunYomi 
                + Environment.NewLine
                + "Meaning: " + App.CurrentKanji.Meaning
                + Environment.NewLine
                + "JLPT: " + App.CurrentKanji.JLPTLevel.ToString()
                + Environment.NewLine
                + "Stroke Count: " + App.CurrentKanji.StrokeCount
                + Environment.NewLine
                + Environment.NewLine
                + "JLPT Setting: " + App.AppSettings.JLPTLevels
            };
            emailComposeTask.Show();
        }

        //private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{

        //    foreach (Storyboard s in phoneApplicationPage.Resources.Values.OfType<System.Windows.Media.Animation.Storyboard>()) {
        //        s.SetValue(Storyboard.TargetNameProperty, "ReadingScrollViewer");
        //    }
        //}
    }
}