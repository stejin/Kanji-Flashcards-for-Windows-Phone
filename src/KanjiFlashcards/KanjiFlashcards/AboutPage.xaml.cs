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

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var emailComposeTask = new EmailComposeTask {
                Subject = "Support Kanji Flashcards for Windows Phone",
                Body = "Please send your donation in Bitcoin to: " + Environment.NewLine + BitcoinWallet.Text
                + Environment.NewLine
                + Environment.NewLine
                + "Thank you!"
            };
            emailComposeTask.Show();
        }
        
    }
}