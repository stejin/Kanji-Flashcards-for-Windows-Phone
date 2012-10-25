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
using KanjiFlashcards.ViewModels;

namespace KanjiFlashcards.Core
{
    public delegate void KanjiListEventHandler(object sender, KanjiListEventArgs e);
    
    public class KanjiListEventArgs : EventArgs
    {

        public KanjiListViewModel ViewModelInstance { get; set;  }

    }
}
