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
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows.Resources;
using NeoDatis.Odb;
using KanjiDatabase;

namespace GenerateKanjiDatabase
{
    public class Database
    {
        private const string baseFilePath = "Data/kanjidic2.neodatis";
        private const string kanjiDatabaseUri = @"http://test.stejin.org/wp7/files/kanjidic2.neodatis.zip";
        private const string connectionString = @"isostore:/KanjiDB.sdf";

        public event EventHandler DatabaseUpdateCompleted;

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

            }
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


        private String GetDirectory(string fileName)
        {
            if (fileName.Contains("/"))
                return fileName.Substring(0, fileName.LastIndexOf('/'));
            else
                return string.Empty;
        }

        private bool KanjiDatabaseExists()
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

        private String GetFileName(string fileName)
        {
            if (fileName.Contains("/"))
                return fileName.Substring(fileName.LastIndexOf('/') + 1);
            else
                return fileName;
        }

        public void CopyData()
        {
            using (KanjiDataContext context = new KanjiDataContext(connectionString)) {

                if (context.DatabaseExists())
                    context.DeleteDatabase();
                context.CreateDatabase();

                using (var odb = ODBFactory.Open(baseFilePath)) {
                    var data = odb.GetObjects<KanjiData>();

                    foreach (var kanji in data) {
                        context.Kanji.InsertOnSubmit(new Kanji() { Id = kanji.Id, JLPTLevel = kanji.JLPTLevel, KunYomi = kanji.KunYomi, Literal = kanji.Literal, Meaning = kanji.Meaning, OnYomi = kanji.OnYomi, StrokeCount = kanji.StrokeCount });
                    }

                    context.SubmitChanges();

                }
            }


        }
    }
}
