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
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;

namespace KanjiFlashcards.Core
{
    public class ReviewList
    {
        public List<string> KanjiList { get; set; }

        public void Save()
        {
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream stream = iso.CreateFile("ReviewList.xml");
            StreamWriter writer = new StreamWriter(stream);
            XmlSerializer ser = new XmlSerializer(typeof(ReviewList));
            ser.Serialize(writer, this);
            writer.Close();
            iso.Dispose();
        }

        public static ReviewList Load()
        {
            ReviewList kanjiList;
            IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication();
            if (iso.FileExists("ReviewList.xml")) {
                IsolatedStorageFileStream stream = iso.OpenFile("ReviewList.xml", FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                try {
                    XmlSerializer ser = new XmlSerializer(typeof(ReviewList)); kanjiList = ser.Deserialize(reader) as ReviewList;
                } catch (Exception ex) {
                    kanjiList = new ReviewList();
                    kanjiList.KanjiList = new List<string>();
                } finally {
                    reader.Close();
                }
            } else {
                kanjiList = new ReviewList();
                kanjiList.KanjiList = new List<string>();
            }
            iso.Dispose();
            return kanjiList;
        }
    }
}
