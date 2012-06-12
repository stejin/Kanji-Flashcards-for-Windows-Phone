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
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Resources;

namespace SilverlightTestApplication
{
    public class DeployUtility
    {
        private const string baseFilePath = "Data/kanjidic2.neodatis";


        private void DeployDatabase(Stream source, bool replace = false)
        {
            DeleteKanjiDatabase();
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication()) {
                if (!isf.FileExists(baseFilePath) || replace == true) {
                    if (isf.Quota < (1024 * 1024 * 8))
                        isf.IncreaseQuotaTo(1024 * 1024 * 8);
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
            DeployDatabase(GetFileStream("Data/kanjidic2.neodatis"), true);
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


        private Stream GetFileStream(string filename)
        {
            Uri fileUri = new Uri(filename, UriKind.Relative);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(fileUri);

            if (stream != null) {
                return stream.Stream;
            }
            return null;
        }

    }
}
