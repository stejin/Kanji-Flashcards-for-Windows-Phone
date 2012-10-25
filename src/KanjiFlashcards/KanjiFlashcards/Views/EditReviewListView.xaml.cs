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
using KanjiFlashcards.Core;

namespace KanjiFlashcards.Views
{
    public partial class EditReviewListView : PhoneApplicationPage
    {

        private ReviewListViewModel vm;

        public EditReviewListView()
        {
            InitializeComponent();

            vm = new ReviewListViewModel();
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            vm.GetKanjiLists();
            this.DataContext = vm.CurrentReviewList;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Visibility = System.Windows.Visibility.Visible;
            var result = vm.SaveKanjiLists();
            if (result.Result == Result.Failure) {
                progressBar.Visibility = System.Windows.Visibility.Collapsed;
                MessageBox.Show(result.Message);
                return;
            }
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            this.NavigationService.GoBack();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}