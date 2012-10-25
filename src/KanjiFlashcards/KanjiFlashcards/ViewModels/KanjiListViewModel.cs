using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.ComponentModel;
using KanjiFlashcards.Core;

namespace KanjiFlashcards.ViewModels
{
    public class KanjiListViewModel : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value) {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public List<string> Kanji { get; set; }

        private int bookmark;
        public int Bookmark
        {
            get
            {
                if (bookmark == 0 && Count > 0)
                    return 1;
                return bookmark;
            }
            set
            {
                    bookmark = value;
            }
        }

        public int Count
        {
            get
            {
                if (Kanji == null)
                    return 0;
                return Kanji.Count;
            }
        }

        private bool isCompleted = false;
        public bool Completed
        {
            get
            {
                return isCompleted;
            }
            set
            {
                if (isCompleted != value) {
                    isCompleted = value;
                    RaisePropertyChanged("Completed");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
 
    }
}
