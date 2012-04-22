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
using Microsoft.Phone.Net.NetworkInformation;
using KanjiFlashcards.ServiceContracts.Operations;
using System.Xml.Serialization;

namespace KanjiFlashcards
{
    public class PasswordValidator
    {
        public event EventHandler<PasswordValidatedEventArgs> PasswordValidated;

        public void ValidatePassword(string password)
        {
            if (NetworkInterface.GetIsNetworkAvailable()) {

                App.AppSettings.DeveloperPassword = password;

                WebClient client = new WebClient();
                client.OpenReadCompleted += new OpenReadCompletedEventHandler(CheckPassword);
                client.OpenReadAsync(new Uri(App.BaseUrl + "/TestDevPassword?" + App.AppSettings.DeveloperPassword));
            }
        }

        private void CheckPassword(object sender, OpenReadCompletedEventArgs args)
        {
            try {
                var serializer = new XmlSerializer(typeof(TestDevPasswordResponse));
                TestDevPasswordResponse response = serializer.Deserialize(args.Result) as TestDevPasswordResponse;

                App.AppSettings.IsExperimentalFunctionalityEnabled = response.IsValid;
                
            } catch { }
            if (App.AppSettings.IsExperimentalFunctionalityEnabled != null) {
                var handler = PasswordValidated;
                if (handler != null)
                    handler(this, new PasswordValidatedEventArgs(App.AppSettings.IsExperimentalFunctionalityEnabled));
            }
        }
    }

    

    public class PasswordValidatedEventArgs : EventArgs
    {
        public PasswordValidatedEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        public bool IsValid { get; private set; }
    }
}
