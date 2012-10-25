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
using System.ComponentModel;

namespace KanjiFlashcards.ViewModels
{
    public class LookupMessageViewModel : INotifyPropertyChanged
    {
 
        private string itemToLookup;
        public string ItemToLookup
        {
            get
            {
                return itemToLookup;
            }
            set
            {
                if (itemToLookup != value) {
                    itemToLookup = value;
                    RaisePropertyChanged("ItemToLookup");
                }
            }
        }

        private string lookupResult;
        public string LookupResult
        {
            get
            {
                return lookupResult;
            }
            set
            {
                if (lookupResult != value) {
                    lookupResult = value;
                    RaisePropertyChanged("LookupResult");
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
