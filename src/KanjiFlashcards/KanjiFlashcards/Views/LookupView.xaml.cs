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
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using KanjiFlashcards.Core;
using System.Text;
using KanjiDatabase;
using System.ComponentModel;
using KanjiFlashcards.ViewModels;

namespace KanjiFlashcards.Views
{
    public partial class LookupView : PhoneApplicationPage
    {
        private bool isLoading;
        private LookupViewModel vm;
        
        public LookupView()
        {
            InitializeComponent();
            isLoading = false;
            vm = new LookupViewModel();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            progressBar.Visibility = System.Windows.Visibility.Collapsed;
            vm.Load();
            this.DataContext = vm;
          //  LookupMessageListBox.ItemsSource = vm.LookupMessages;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (isLoading) {
                return;
            }
            isLoading = true;

            progressBar.Visibility = System.Windows.Visibility.Visible;

            App.KanjiMode = Mode.Lookup;

            var worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(ParseAsync);
            worker.WorkerReportsProgress = false;
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ParseCompleted);
            worker.RunWorkerAsync();
        }

        private void ParseAsync(object sender, DoWorkEventArgs e)
        {
            vm.Parse();         
        }

        private void ParseCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = vm.Save();

            isLoading = false;
            progressBar.Visibility = System.Windows.Visibility.Collapsed;

            if (result.Result == Result.Failure) {
                MessageBox.Show(result.Message);
            } else {
                this.NavigationService.Navigate(new Uri("/Views/KanjiView.xaml", UriKind.Relative));
            }
        }

     }
}