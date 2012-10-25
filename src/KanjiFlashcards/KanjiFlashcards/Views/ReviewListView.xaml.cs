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
using KanjiFlashcards.ViewModels;
using KanjiFlashcards.Views;
using KanjiFlashcards.Core;
using System.ComponentModel;

namespace KanjiFlashcards.Views
{
    public partial class ReviewListView : PhoneApplicationPage
    {
        private ReviewListViewModel vm;

        private bool isLoading;

        private IReviewListView view;

        public ReviewListView()
        {
            InitializeComponent();
            vm = new ReviewListViewModel();
            isLoading = false;
            SwipeDown.Completed += SwipeDown_Completed;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.CurrentKanji != null)
                SwipeUp.Begin();

            ContentStackPanel.Children.Clear();
            

            if (e.Uri.ToString().EndsWith("load")) {
                if (App.AppSettings.IsRandomReviewList)
                    view = new LoadFromReviewListRandomView();
                else 
                    view = new LoadFromReviewListSequentialView();
                view.ItemSelected += SelectedForLoad;
                ApplicationBar.IsVisible = false;
            }


            if (e.Uri.ToString().EndsWith("add")) {
                view = new SaveToReviewListView();
                view.ItemSelected += SelectedForAdd;
                ApplicationBar.IsVisible = true;
            }

            view.DataContext = vm.GetKanjiLists();
            view.SelectedForEdit += SelectedForEdit;
            view.SelectedForDelete += SelectedForDelete;
            ContentStackPanel.Children.Add(view as UserControl);
        
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            var result = vm.SaveKanjiLists();

            switch (result.Result) {
                case Result.Success:
                    return;
                case Result.Failure:
                    MessageBox.Show(String.Format("Unable to save review lists: {0}.", result.Message));
                    e.Cancel = true;
                    break;
            }
        }

        private void SelectedForLoad(object sender, KanjiListEventArgs e)
        {
            if (isLoading) {
                return;
            }
            isLoading = true;

            progressBar.Visibility = System.Windows.Visibility.Visible;

            var result = vm.SaveKanjiLists();
            if (result.Result == Result.Failure) {
                isLoading = false;
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show(result.Message);
                return;
            }

            vm.CurrentReviewList = e.ViewModelInstance;
            App.KanjiMode = Mode.Review;

            if (!App.AppSettings.IsRandomReviewList && e.ViewModelInstance.Bookmark > 0) {
                App.IsResumeKanji = true;
                App.CurrentKanji = App.KanjiDict.GetKanjiFromDatabase(e.ViewModelInstance.Kanji[e.ViewModelInstance.Bookmark - 1]);
            }

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(LoadReviewListAsync);
            worker.WorkerReportsProgress = false;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadKanjiCompleted);
            worker.RunWorkerAsync();
        }

        private void SelectedForDelete(object sender, KanjiListEventArgs e)
        {
            vm.DeleteReviewList(e.ViewModelInstance);
        }

        private void SelectedForEdit(object sender, KanjiListEventArgs e)
        {
            vm.CurrentReviewList = e.ViewModelInstance;
            this.NavigationService.Navigate(new Uri("/Views/EditReviewListView.xaml", UriKind.Relative));
        }

        private void SelectedForAdd(object sender, KanjiListEventArgs e)
        {
            if (isLoading) {
                return;
            }
            isLoading = true;

            progressBar.Visibility = System.Windows.Visibility.Visible;

            var result = vm.AddKanjiToReviewList(e.ViewModelInstance, App.CurrentKanji.Literal);
            if (result.Result == Result.Failure) {
                isLoading = false;
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show(result.Message);
                return;
            }

            result = vm.SaveKanjiLists();

            isLoading = false;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;

            if (result.Result == Result.Failure) {
                MessageBox.Show(result.Message);
                return;
            }

            vm.CurrentReviewList = e.ViewModelInstance;
            App.IsResumeKanji = true;

            SwipeDown.Begin();
        }

      
        private void LoadReviewListAsync(object sender, DoWorkEventArgs e)
        {
           App.KanjiDict.LoadFromDatabase(App.CurrentReviewList.Kanji);
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

        private void AddNew(object sender, EventArgs e)
        {
            vm.AddNewReviewList();
            this.NavigationService.Navigate(new Uri("/Views/EditReviewListView.xaml", UriKind.Relative));
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if ((App.KanjiMode == Mode.Flashcards || App.KanjiMode == Mode.Lookup) && App.CurrentKanji != null) {
                SwipeDown.Begin();
                e.Cancel = true;
            } else {
                base.OnBackKeyPress(e);
            }
        }

        private void SwipeDown_Completed(object sender, EventArgs args)
        {
            this.NavigationService.GoBack();
        }

    }
}