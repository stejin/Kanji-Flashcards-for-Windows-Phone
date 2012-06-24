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
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using NeoDatis.Odb;
using NeoDatis.Odb.Core.Query;
using NeoDatis.Odb.Impl.Core.Query.Criteria;
using KanjiDatabase;
using NeoDatis.Odb.Core.Query.Criteria;

namespace KanjiFlashcards.Core
{
    public class KanjiDictionary
    {
        private Dictionary<string, Kanji> kanjiDictionary;

        private List<int> sequence;

        private const string baseFilePath = "Data/kanjidic2.neodatis";
        private const string kanjiDatabaseUri = @"http://test.stejin.org/wp7/files/kanjidic2.neodatis.zip";

        public event EventHandler DatabaseUpdateCompleted;
        public event EventHandler<DatabaseUpdateErrorEventArgs> DatabaseUpdateError;

        public int KanjiCount
        {
            get
            {
                if (kanjiDictionary != null)
                    return kanjiDictionary.Count;
                return 0;
            }
        }

        public void LoadFromDatabase(JLPT jlptLevels)
        {
            ODB odb = ODBFactory.Open(baseFilePath);

            try {

                kanjiDictionary = new Dictionary<string, Kanji>();

                JLPT jlpt1 = JLPT.Undefined;
                JLPT jlpt2 = JLPT.Undefined;
                JLPT jlpt3 = JLPT.Undefined;
                JLPT jlpt4 = JLPT.Undefined;
                JLPT jlptOther = JLPT.Undefined;

                if ((jlptLevels & JLPT.Level1) == JLPT.Level1)
                    jlpt1 = JLPT.Level1;
                    
                if ((jlptLevels & JLPT.Level2) == JLPT.Level2)
                    jlpt2 = JLPT.Level2;

                if ((jlptLevels & JLPT.Level3) == JLPT.Level3)
                    jlpt3 = JLPT.Level3;

                if ((jlptLevels & JLPT.Level4) == JLPT.Level4)
                    jlpt4 = JLPT.Level4;

                if ((jlptLevels & JLPT.Other) == JLPT.Other)
                    jlptOther = JLPT.Other;

                IQuery query = new CriteriaQuery(typeof(KanjiData), Where.Or()
                    .Add(Where.Equal("JLPTLevel", jlpt1.ToString()))
                    .Add(Where.Equal("JLPTLevel", jlpt2.ToString()))
                    .Add(Where.Equal("JLPTLevel", jlpt3.ToString()))
                    .Add(Where.Equal("JLPTLevel", jlpt4.ToString()))
                    .Add(Where.Equal("JLPTLevel", jlptOther.ToString())));
                var data = odb.GetObjects<KanjiDatabase.KanjiData>(query);
                data.ToList<KanjiData>().ForEach(k => kanjiDictionary.Add(k.Literal, new Kanji(k)));
                if (App.AppSettings.IsRandomFlashcards)
                    GenerateRandomSequence();
                else
                    GenerateSequence();
            } catch {

            } finally {
                odb.Close();
            }
        }

        public void LoadFromDatabase(List<string> reviewList)
        {
            ODB odb = ODBFactory.Open(baseFilePath);

            try {
                kanjiDictionary = new Dictionary<string, Kanji>();

                foreach (string literal in reviewList) {
                    IQuery query = new CriteriaQuery(typeof(KanjiData), Where.Equal("Literal", literal));
                    var data = odb.GetObjects<KanjiDatabase.KanjiData>(query);
                    if (data.GetFirst() != null)
                        kanjiDictionary.Add(literal, new Kanji(data.GetFirst()));
                }

                if (App.AppSettings.IsRandomReviewList)
                    GenerateRandomSequence();
                else
                    GenerateSequence();

            } catch {

            } finally {
                odb.Close();
            }
        }

        private void GenerateSequence()
        {
            sequence = new List<int>(kanjiDictionary.Count);

            List<int> temp = new List<int>(kanjiDictionary.Count);

            for (int i = 0; i < kanjiDictionary.Count; i++)
                sequence.Add(i);
        }

        private void GenerateRandomSequence()
        {
            sequence = new List<int>(kanjiDictionary.Count);

            List<int> temp = new List<int>(kanjiDictionary.Count);

            for (int i = 0; i < kanjiDictionary.Count; i++)
                temp.Add(i);

            Random rand = new Random(DateTime.Now.Millisecond);

            while (sequence.Count < kanjiDictionary.Count) {
                int index = rand.Next(0, temp.Count);
                sequence.Add(temp[index]);
                temp.RemoveAt(index);
            }
        }

        public Kanji GetKanji(int index)
        {
            return kanjiDictionary.Values.ElementAt<Kanji>(sequence[index]);
        }

        public bool IsIndexValid(int index)
        {
            return (index > -1 && index < kanjiDictionary.Count);
        }

        public int GetKanjiIndex(string kanji)
        {
            if (kanjiDictionary.ContainsKey(kanji))
                return sequence.IndexOf(kanjiDictionary.Keys.ToList().IndexOf(kanji));
            else
                return 0;
        }

        private void DeployDatabase(Stream source, bool replace = false)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication()) {
                if (!isf.FileExists(baseFilePath) || replace == true) {
                    DeleteKanjiDatabase();
                    BinaryReader fileReader = new BinaryReader(source);
                    if (GetDirectory(baseFilePath) != string.Empty && !isf.DirectoryExists(GetDirectory(baseFilePath)))
                        isf.CreateDirectory(GetDirectory(baseFilePath));
                    IsolatedStorageFileStream outFile = isf.CreateFile(baseFilePath);
                    bool eof = false;
                    long fileLength = fileReader.BaseStream.Length;
                    int writeLength = 512;
                    while (!eof) {
                        if (fileLength < 512) {
                            writeLength = Convert.ToInt32(fileLength);
                            outFile.Write(fileReader.ReadBytes(writeLength), 0, writeLength);
                        } else {
                            outFile.Write(fileReader.ReadBytes(writeLength), 0, writeLength);
                        }
                        fileLength = fileLength - 512;
                        if (fileLength <= 0) eof = true;
                    }
                    fileReader.Close();
                    outFile.Close();
                }
            }
        }

        private void DeleteKanjiDatabase()
        {
            if (IsolatedStorageFile.GetUserStoreForApplication().FileExists(baseFilePath))
                IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(baseFilePath);
        }

        public void UpdateKanjiDatatabaseFromXap()
        {
            DeployDatabase(GetFileStream("Dictionary/kanjidic2.neodatis"), true);
        }

        public void UpdateKanjiDatatabaseFromInternet()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(DatabaseDownloadCompleted);
            client.OpenReadAsync(new Uri(kanjiDatabaseUri));
        }

        private void DatabaseDownloadCompleted(object sender, OpenReadCompletedEventArgs args)
        {
            try {
                DeployDatabase(GetUncompressedStream(args.Result), true);
                EventHandler handler = DatabaseUpdateCompleted;
                if (handler != null)
                    handler(this, new EventArgs());
            } catch (Exception e) {
                var handler = DatabaseUpdateError;
                if (handler != null) {
                    handler(this, new DatabaseUpdateErrorEventArgs() { ErrorMessage = e.Message });
                }
            }
        }

        public bool KanjiDatabaseExists()
        {
            return IsolatedStorageFile.GetUserStoreForApplication().FileExists(baseFilePath);
        }

        private Stream GetFileStream(string filename)
        {
            Uri fileUri = new Uri(filename, UriKind.Relative);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(fileUri);

            if (stream != null) {
                return stream.Stream;
            }
            return null;
        }

        private Stream GetUncompressedStream(Stream source)
        {
            Uri fileUri = new Uri(GetFileName(baseFilePath), UriKind.Relative);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(new StreamResourceInfo(source, "zip"), fileUri);

            if (stream != null) {
                return stream.Stream;
            }
            return null;
        }

        public String GetDirectory(string fileName)
        {
            if (fileName.Contains("/"))
                return fileName.Substring(0, fileName.LastIndexOf('/'));
            else
                return string.Empty;
        }

        public String GetFileName(string fileName)
        {
            if (fileName.Contains("/"))
                return fileName.Substring(fileName.LastIndexOf('/') + 1);
            else
                return fileName;
        }

    }
}
