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
using KanjiDatabase;
using System.Windows.Media.Imaging;
using KanjiFlashcards.Core;
using System.ComponentModel;

namespace KanjiFlashcards.ViewModels
{
    public class KanjiViewModel : INotifyPropertyChanged
    {

        private Kanji kanji;

        private ICommand moveNextCommand;
        private ICommand movePreviousCommand;
        private ICommand moveLastCommand;
        private ICommand moveFirstCommand;
        private ICommand addItemToReviewListCommand;
        private ICommand removeItemFromReviewListCommand;
        private ICommand reportBrokenKanjiCommand;

        public event EventHandler OnMoveForward;
        public event EventHandler OnMoveBackward;
        public event EventHandler OnAddItemToReviewList;
        public event EventHandler OnRemoveItemFromReviewList;
        public event EventHandler OnReportBrokenKanji;
    
        public KanjiViewModel()
        {
            moveNextCommand = new DelegateCommand(MoveForwardAction);
            movePreviousCommand = new DelegateCommand(MovePreviousAction);
            moveLastCommand = new DelegateCommand(MoveLastAction);
            moveFirstCommand = new DelegateCommand(MoveFirstAction);
            addItemToReviewListCommand = new DelegateCommand(AddItemToReviewListAction);
            removeItemFromReviewListCommand = new DelegateCommand(RemoveItemFromReviewListAction);
            reportBrokenKanjiCommand = new DelegateCommand(ReportBrokenKanjiAction);
            (addItemToReviewListCommand as DelegateCommand).IsCanExecute = App.KanjiMode == Mode.Flashcards || App.KanjiMode == Mode.Lookup;
            (removeItemFromReviewListCommand as DelegateCommand).IsCanExecute = App.KanjiMode == Mode.Review;
            (reportBrokenKanjiCommand as DelegateCommand).IsCanExecute = true;
        }
       
        private int index;
        public int Index
        {
            get { return index; }
            set
            {
                index = value;
                if (App.KanjiMode == Core.Mode.Review)
                    App.CurrentReviewList.Bookmark = Index + 1;
                kanji = App.KanjiDict.GetKanji(index);
                App.CurrentKanji = kanji;
            }
        }

        public string Id
        {
            get { return "Id: " + kanji.Id; }
        }

        public string Literal
        {
            get { return kanji.Literal; }
        }

        public string Meaning
        {
            get { return kanji.Meaning; }
        }

        public string OnYomi
        {
            get { return kanji.OnYomi; }
        }

        public string KunYomi
        {
            get { return kanji.KunYomi; }
        }

        public string JLPTLevel
        {
            get { return "JLPT Level: " + kanji.JLPTLevel.ToString().Replace("Level", ""); }
        }

        public string Position
        {
            get { return String.Format("{0} / {1}", Index + 1, GetMaxIndex() + 1);  }
        }

        public string StrokeCount
        {
            get { return "Strokes: " + kanji.StrokeCount; }
        }

        public ImageSource Image
        {
            get { return new BitmapImage(new Uri(String.Format("/Images/Kanji/{0}.png", kanji.Id), UriKind.Relative)); }
        }

        public bool HasCurrent
        {
            get {return App.KanjiDict.IsIndexValid(Index); }
        }

        public bool HasNext
        {
            get { return App.KanjiDict.IsIndexValid(Index + 1); }
        }

        public bool HasPrevious
        {
            get { return App.KanjiDict.IsIndexValid(Index - 1); }
        }

        public int GetMaxIndex()
        {
            return App.KanjiDict.KanjiCount - 1;
        }

        private void LoadPreviousAction(object p)
        {
            Index -= 1;
        }

        private void LoadLastAction(object p)
        {
            Index = GetMaxIndex();
        }

        private void LoadFirstAction(object p)
        {
            Index = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null) {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void NotifyPropertyChanges()
        {
            RaisePropertyChanged("Id");
            RaisePropertyChanged("Position");
            RaisePropertyChanged("Image");
            RaisePropertyChanged("Literal");
            RaisePropertyChanged("Meaning");
            RaisePropertyChanged("OnYomi");
            RaisePropertyChanged("KunYomi");
            RaisePropertyChanged("JLPTLevel");
            RaisePropertyChanged("StrokeCount");
            RaisePropertyChanged("MoveNextCommand");
            RaisePropertyChanged("MovePreviousCommand");
            RaisePropertyChanged("MoveFirstCommand");
            RaisePropertyChanged("MoveLastCommand");
            RaisePropertyChanged("HasCurrent");
            RaisePropertyChanged("HasNext");
            RaisePropertyChanged("HasPrevious");
        }

        public ICommand MoveNextCommand
        {
            get {
                (moveNextCommand as DelegateCommand).IsCanExecute = HasNext;
                return moveNextCommand; 
            }
        }

        public ICommand MovePreviousCommand
        {
            get {
                (movePreviousCommand as DelegateCommand).IsCanExecute = HasPrevious;
                return movePreviousCommand; 
            }
        }

        public ICommand MoveLastCommand
        {
            get {
                (moveLastCommand as DelegateCommand).IsCanExecute = HasNext;
                return moveLastCommand; 
            }
        }

        public ICommand MoveFirstCommand
        {
            get {
                (moveFirstCommand as DelegateCommand).IsCanExecute = HasPrevious;
                return moveFirstCommand; 
            }
        }

        private void MoveForwardAction(object p)
        {
            if (OnMoveForward != null)
                OnMoveForward(p, new EventArgs());
            Index += 1;
        }

        private void MovePreviousAction(object p)
        {
            if (OnMoveBackward != null)
                OnMoveBackward(p, new EventArgs());
            Index -= 1;
        }

        private void MoveLastAction(object p)
        {
            if (OnMoveForward != null)
                OnMoveForward(p, new EventArgs());
            Index = GetMaxIndex();
        }

        private void MoveFirstAction(object p)
        {
            if (OnMoveBackward != null)
                OnMoveBackward(p, new EventArgs());
            Index = 0;
        }

        public ICommand AddItemToReviewListCommand
        {
            get { return addItemToReviewListCommand; }
        }

        public ICommand RemoveItemFromReviewListCommand
        {
            get { return removeItemFromReviewListCommand; }
        }

        public ICommand ReportBrokenKanjiCommand
        {
            get { return reportBrokenKanjiCommand; }
        }

        private void AddItemToReviewListAction(object p)
        {
            if (OnAddItemToReviewList != null)
                OnAddItemToReviewList(p, new EventArgs());
        }

        private void RemoveItemFromReviewListAction(object p)
        {
            if (OnRemoveItemFromReviewList != null)
                OnRemoveItemFromReviewList(p, new EventArgs());
        }

        private void ReportBrokenKanjiAction(object p)
        {
            if (OnReportBrokenKanji != null)
                OnReportBrokenKanji(p, new EventArgs());
        }

        public void RemoveItemFromReviewList()
        {
            App.KanjiDict.RemoveKanjiFromDictionary(Literal);
            App.CurrentReviewList.Kanji.Remove(Literal);
            if (HasCurrent)
                Index = Index;
            else if (GetMaxIndex() > -1)
                Index = GetMaxIndex();
            else {
                kanji = null;
                App.CurrentKanji = null;
                App.CurrentReviewList.Bookmark = 0;
            }
            App.ReviewList.Save();
        }

    }

}

