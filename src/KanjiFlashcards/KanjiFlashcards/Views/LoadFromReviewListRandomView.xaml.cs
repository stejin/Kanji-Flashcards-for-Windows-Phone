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
using KanjiFlashcards.Core;
using System.ComponentModel;
using KanjiFlashcards.ViewModels;
using System.Collections.ObjectModel;

namespace KanjiFlashcards.Views
{
    public partial class LoadFromReviewListRandomView : UserControl, IReviewListView
    {

        public LoadFromReviewListRandomView()
        {
            InitializeComponent();
        }

        #region "Review List View"

        public event KanjiListEventHandler ItemSelected;
        public event KanjiListEventHandler SelectedForEdit;
        public event KanjiListEventHandler SelectedForDelete;

        private void NameTextBox_Tap(object sender, GestureEventArgs e)
        {
            if (ItemSelected != null) {
                ItemSelected(sender, new KanjiListEventArgs() { ViewModelInstance = (sender as TextBlock).DataContext as KanjiListViewModel });
            }
        }

        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedForEdit != null) {
                this.SelectedForEdit(sender, new KanjiListEventArgs() { ViewModelInstance = (sender as Microsoft.Phone.Controls.MenuItem).DataContext as KanjiListViewModel });
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedForDelete != null) {
                this.SelectedForDelete(sender, new KanjiListEventArgs() { ViewModelInstance = (sender as Microsoft.Phone.Controls.MenuItem).DataContext as KanjiListViewModel });
            }

        }
      
        #endregion
    }
}
