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
using KanjiFlashcards.Core;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace KanjiFlashcards
{
    public class ApplicationSettings
    {
        public string KanjiImageBaseUrl = @"http://app.stejin.org/wp7/images/kanji/";

        public string TodayKanjiImageBaseUrl = @"http://app.stejin.org/wp7/images/todaykanji/";

        public int JLPTLevels { get; set; }

        public bool IsRandomFlashcards { get; set; }
        
        public bool IsRandomReviewList { get; set; }

        public int DatabaseVersion { get; set; }

        public KanjiFlashcards.ServiceContracts.Types.KanjiMessage TodayKanji { get; set; }

        private string developerPassword;
        public string DeveloperPassword
        {
            get
            {
                if (String.IsNullOrEmpty(developerPassword))
                    return "";
                else
                    return developerPassword;
            }
            set { developerPassword = value; }
        }

        public bool IsExperimentalFunctionalityEnabled { get; set; }

        public JLPT GetJLPTLevels()
        {
            return (JLPT)JLPTLevels;
        }

        public void SetJLPTLevels(JLPT value)
        {
            JLPTLevels = (int)value;
        }

        public void Save()
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = iso.CreateFile("Settings.xml");
            StreamWriter writer = new StreamWriter(stream);
            XmlSerializer ser = new XmlSerializer(typeof(ApplicationSettings));
            ser.Serialize(writer, this);
            writer.Close();
            iso.Dispose();
        }

        public static ApplicationSettings Load()
        {
            ApplicationSettings settings;
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            if (iso.FileExists("Settings.xml")) {
                IsolatedStorageFileStream stream = iso.OpenFile("Settings.xml", FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(ApplicationSettings)); settings = ser.Deserialize(reader) as ApplicationSettings;
                } catch (Exception ex) {
                    settings = new ApplicationSettings();
                    settings.SetJLPTLevels(JLPT.Level2 | JLPT.Level3 | JLPT.Level4);
                    settings.IsRandomFlashcards = true;
                    settings.IsRandomReviewList = true;
                    settings.DatabaseVersion = 0;
                    settings.DeveloperPassword = "";
                    settings.IsExperimentalFunctionalityEnabled = false;
                } finally {
                    reader.Close();
                }
            } else {
                settings = new ApplicationSettings();
                settings.SetJLPTLevels(JLPT.Level2 | JLPT.Level3 | JLPT.Level4);
                settings.IsRandomFlashcards = true;
                settings.IsRandomReviewList = true;
                settings.DatabaseVersion = 0;
                settings.DeveloperPassword = "";
                settings.IsExperimentalFunctionalityEnabled = false;
            }
            iso.Dispose();
            if (settings.TodayKanji == null)
                settings.TodayKanji = new KanjiFlashcards.ServiceContracts.Types.KanjiMessage() { Id = 409, Literal = "漢", KunYomi = "", OnYomi = "カン", Meaning = "Sino-, China", Jlpt = KanjiFlashcards.ServiceContracts.Types.JLPT.Level3, StrokeCount = 13 };
            return settings;
        }
    }
}
