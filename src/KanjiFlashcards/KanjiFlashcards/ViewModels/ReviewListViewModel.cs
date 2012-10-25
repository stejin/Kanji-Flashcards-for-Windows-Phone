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
using System.Collections.ObjectModel;
using KanjiFlashcards.Core;
using System.Linq;

namespace KanjiFlashcards.ViewModels
{
    public class ReviewListViewModel
    {

        private ObservableCollection<KanjiListViewModel> kanjiLists;

        private bool isDirty;

        public ObservableCollection<KanjiListViewModel> GetKanjiLists()
        {
            kanjiLists = new ObservableCollection<KanjiListViewModel>();

            App.ReviewList.KanjiLists.ForEach(k => kanjiLists.Add(new KanjiListViewModel() { Name = k.Name, Kanji = k.Kanji, Bookmark = k.Bookmark, Completed = k.Completed }));
            kanjiLists.ToList().ForEach(k => k.PropertyChanged += ReviewListChanged);
            isDirty = false;
            return kanjiLists;
        }

        public void AddReviewList(string name)
        {
            var kanjiList = new KanjiListViewModel() { Name = name, Kanji = new List<string>(), Bookmark = 0, Completed = false };
            kanjiList.PropertyChanged += ReviewListChanged;
            kanjiLists.Add(kanjiList);
            isDirty = true;
            SaveKanjiLists();
            CurrentReviewList = kanjiList;
        }

        public void AddNewReviewList()
        {
            int i = kanjiLists.Count() + 1;
            string name = "List {0}";
            while (kanjiLists.Count(k => k.Name == String.Format(name, i)) > 0) {
                i++;
            }
            AddReviewList(String.Format(name, i));
        }

        public void DeleteReviewList(KanjiListViewModel kanjiList)
        {
            kanjiLists.Remove(kanjiList);
            isDirty = true;
        }


        private void ReviewListChanged(object sender, EventArgs e)
        {
            isDirty = true;
        }

        public OperationResult SaveKanjiLists()
        {
            try {

                if (isDirty) {

                    if (kanjiLists.GroupBy(k => k.Name).Count() != kanjiLists.Count())
                        return new OperationResult() { Result = Result.Failure, Message = "Review list names must be unique." };

                    if (kanjiLists.Count(k => k.Name.Trim() == String.Empty) > 0 )
                        return new OperationResult() { Result = Result.Failure, Message = "Review list names cannot be left blank." };

                    var result = new List<KanjiList>();

                    kanjiLists.OrderBy(k => k.Name).ToList().ForEach(k => result.Add(new KanjiList() { Name = k.Name, Kanji = k.Kanji, Bookmark = k.Bookmark, Completed = k.Completed }));
                    App.ReviewList.KanjiLists = result;
                    App.ReviewList.Save();
                    
                    isDirty = false;
                }

            } catch (Exception e) {
                return new OperationResult() { Result = Result.Failure, Message = e.Message };
            }

            return new OperationResult() { Result = Result.Success };
        }

        public KanjiListViewModel CurrentReviewList
        {
            get { return kanjiLists.First(k => k.Name == App.CurrentReviewList.Name); }
            set { App.CurrentReviewList = GetKanjiList(value.Name); }
        }

        private KanjiList GetKanjiList(string name)
        {
            var result = App.ReviewList.KanjiLists.Where(k => k.Name == name);
            if (result.Count() == 0)
                return null;
            return result.First();
        }

        public OperationResult AddKanjiToReviewList(KanjiListViewModel kanjiList, string kanji)
        {
            if (kanjiList.Kanji.Contains(kanji))
                return new OperationResult() { Result = Result.Failure, Message = "Current kanji has already been added to this review list.  Select another review list or go back."};
            kanjiList.Kanji.Add(kanji);
            return new OperationResult() { Result = Result.Success };
        }

    }

}
