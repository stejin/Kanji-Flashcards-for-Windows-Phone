using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KanjiFlashcards.Core;
using KanjiFlashcards.ViewModels;
using System.Collections.ObjectModel;

namespace KanjiFlashcards.Views
{
    interface IReviewListView
    {
        event KanjiListEventHandler ItemSelected;
        event KanjiListEventHandler SelectedForEdit;
        event KanjiListEventHandler SelectedForDelete;

        object DataContext { get; set; }
    }
}
