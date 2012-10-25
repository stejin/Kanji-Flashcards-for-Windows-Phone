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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using KanjiFlashcards.Core;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;
using KanjiDatabase;

namespace KanjiFlashcards
{
    public partial class App : Application
    {
        public const string BaseUrl = "http://app.stejin.org/wp7/services";
      
        // Easy access to the root frame
        public PhoneApplicationFrame RootFrame { get; private set; }

        private static ApplicationSettings appSettings;

        // To store the instance for the application lifetimeprivate 
        private ShellTileSchedule shellTileSchedule;  

        public static ApplicationSettings AppSettings
        {
            get
            {
                if (appSettings == null)
                    appSettings = ApplicationSettings.Load();

                return appSettings;
            }
        }

        public static bool IsResumeKanji { get; set; }

        public static Mode KanjiMode { get; set; }

        public static Kanji CurrentKanji;

        public static KanjiList CurrentReviewList;

        public static KanjiDictionary KanjiDict { get; private set; }

        private static ReviewList reviewList;

        public static ReviewList ReviewList
        {
            get
            {
                if (reviewList == null)
                    reviewList = ReviewList.Load();

                return reviewList;
            }
        }

        public static List<string> LookupKanji { get; set; }

        // Constructor
        public App()
        {
            // Global handler for uncaught exceptions. 
            // Note that exceptions thrown by ApplicationBarItem.Click will not get caught here.
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Validate Developer Password
            var passwordValidator = new PasswordValidator();
            passwordValidator.PasswordValidated += CheckPassword;
            passwordValidator.ValidatePassword(AppSettings.DeveloperPassword);

            // Create the shell tile schedule instance 
            CreateShellTileSchedule();

            KanjiDict = new KanjiDictionary();
        }

        /// <summary> 
        /// Create the application shell tile schedule instance 
        /// </summary> 
        private void CreateShellTileSchedule()
        {
            shellTileSchedule = new ShellTileSchedule();
            shellTileSchedule.Recurrence = UpdateRecurrence.Interval;
            //shellTileSchedule.Interval = UpdateInterval.EveryDay;
            shellTileSchedule.Interval = UpdateInterval.EveryHour;
            shellTileSchedule.StartTime = DateTime.Now;
            shellTileSchedule.RemoteImageUri = new Uri(App.AppSettings.TodayKanjiImageBaseUrl + AppSettings.JLPTLevels.ToString() + ".png");
            shellTileSchedule.Start();
        }

        private void CheckPassword(object sender, PasswordValidatedEventArgs args)
        {
            if (!args.IsValid && AppSettings.DeveloperPassword != String.Empty) {
                MessageBox.Show("Experimental functionality will be disabled.", "Developer password expired", MessageBoxButton.OK);
                AppSettings.DeveloperPassword = "";
            }
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            IsResumeKanji = false;
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            var settingsStorage = IsolatedStorageSettings.ApplicationSettings;
            if (settingsStorage.Contains("LastViewedKanji") && settingsStorage["LastViewedKanji"].ToString() != string.Empty) {
                CurrentKanji = App.KanjiDict.GetKanjiFromDatabase(settingsStorage["LastViewedKanji"].ToString());
                if (settingsStorage.Contains("KanjiMode")) 
                    KanjiMode = (Mode)settingsStorage["KanjiMode"];
                else
                    KanjiMode = Mode.Flashcards;
                switch (KanjiMode) {
                    case Mode.Flashcards:
                        KanjiDict.LoadFromDatabase(AppSettings.GetJLPTLevels());
                        break;
                    case Mode.Review:
                        if (settingsStorage.Contains("ReviewList")) {
                            var result = ReviewList.KanjiLists.Where(k => k.Name == settingsStorage["ReviewList"].ToString());
                            if (result.Count() > 0) {
                                CurrentReviewList = result.First();
                                KanjiDict.LoadFromDatabase(CurrentReviewList.Kanji);
                            }
                        }
                        break;
                    case Mode.Lookup:
                        if (settingsStorage.Contains("LookupKanji")) {
                            KanjiDict.LoadFromDatabase(settingsStorage["LookupKanji"] as List<string>);
                        }
                        break;
                }
                IsResumeKanji = true;
            } else {
                IsResumeKanji = false;
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            ReviewList.Save();
            AppSettings.Save();
            var settingsStorage = IsolatedStorageSettings.ApplicationSettings;
            if (CurrentKanji != null) {
                if (settingsStorage.Contains("LastViewedKanji"))
                    settingsStorage["LastViewedKanji"] = CurrentKanji.Literal;
                else
                    settingsStorage.Add("LastViewedKanji", CurrentKanji.Literal);
            }
            if (settingsStorage.Contains("KanjiMode"))
                settingsStorage["KanjiMode"] = KanjiMode;
            else
                settingsStorage.Add("KanjiMode", KanjiMode);
            if (CurrentReviewList != null) {
                if (settingsStorage.Contains("ReviewList"))
                    settingsStorage["ReviewList"] = CurrentReviewList.Name;
                else
                    settingsStorage.Add("ReviewList", CurrentReviewList.Name);
            }
            if (LookupKanji != null) {
                if (settingsStorage.Contains("LookupKanji"))
                    settingsStorage["LookupKanji"] = LookupKanji;
                else
                    settingsStorage.Add("LookupKanji", LookupKanji);
            }
            settingsStorage.Save();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            ReviewList.Save();
            AppSettings.Save();
        }

        // Code to execute if a navigation fails
        void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached) {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached) {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }



        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame() {
                UriMapper = (UriMapper)Resources["UriMapper"]
                // Use a style to use the Template with the TransitioningContentControl
            //    Style = (Style)Resources["Forward"]
            };
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}
