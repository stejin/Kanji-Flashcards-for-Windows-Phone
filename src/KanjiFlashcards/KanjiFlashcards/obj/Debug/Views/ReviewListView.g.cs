﻿#pragma checksum "C:\Users\Steffen\Documents\GitHub\Kanji-Flashcards-for-Windows-Phone\src\KanjiFlashcards\KanjiFlashcards\Views\ReviewListView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "44B7A8FDE4AD74380D28136F62724C5F"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace KanjiFlashcards.Views {
    
    
    public partial class ReviewListView : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Controls.PhoneApplicationPage phoneApplicationPage;
        
        internal System.Windows.Media.Animation.Storyboard SwipeDown;
        
        internal System.Windows.Media.Animation.Storyboard SwipeUp;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal Microsoft.Phone.Controls.PerformanceProgressBar progressBar;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.StackPanel ContentStackPanel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/KanjiFlashcards;component/Views/ReviewListView.xaml", System.UriKind.Relative));
            this.phoneApplicationPage = ((Microsoft.Phone.Controls.PhoneApplicationPage)(this.FindName("phoneApplicationPage")));
            this.SwipeDown = ((System.Windows.Media.Animation.Storyboard)(this.FindName("SwipeDown")));
            this.SwipeUp = ((System.Windows.Media.Animation.Storyboard)(this.FindName("SwipeUp")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.progressBar = ((Microsoft.Phone.Controls.PerformanceProgressBar)(this.FindName("progressBar")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ContentStackPanel = ((System.Windows.Controls.StackPanel)(this.FindName("ContentStackPanel")));
        }
    }
}

