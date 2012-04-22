﻿using System;
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
using System.Reflection;
using Microsoft.Phone.Tasks;

namespace KanjiFlashcards
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            DatabaseVersion.Text = String.Format(DatabaseVersion.Tag.ToString(), App.AppSettings.DatabaseVersion);

            base.OnNavigatedTo(e);
        }

        private void TextBlock_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {
                To = "app@stejin.org",
                Subject = "Kanji Flashcards",
                Body = ""
            };
            emailComposeTask.Show();
            
            e.Handled = true;
            e.Complete();
        }
        
    }
}