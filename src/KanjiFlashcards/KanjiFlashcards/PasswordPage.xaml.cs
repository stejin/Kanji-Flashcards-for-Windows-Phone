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

namespace KanjiFlashcards
{
    public partial class Password : PhoneApplicationPage
    {
        public Password()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            var validator = new PasswordValidator();
            validator.PasswordValidated += CheckPassword;
            validator.ValidatePassword(PasswordTextBox.Password);
        }

        private void CheckPassword(object sender, PasswordValidatedEventArgs args)
        {
            if (args.IsValid) {
                MessageBox.Show("Experimental functionality enabled.", "Password valid", MessageBoxButton.OK);
                this.NavigationService.GoBack();
            } else {
                MessageBox.Show("Experimental functionality disabled.", "Password invalid", MessageBoxButton.OK);
                App.AppSettings.DeveloperPassword = "";
            }
        }

    }
}