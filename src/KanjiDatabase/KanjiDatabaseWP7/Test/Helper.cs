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

namespace Test
{
    public static class Helper
    {

        public static void CopyBinaryFile(this IsolatedStorageFile isf, string source, string target, bool replace = false)
        {
            if (!isf.FileExists(target) || replace == true) {
                BinaryReader fileReader = new BinaryReader(GetFileStream(source));
                if (GetDirectory(target) != string.Empty)
                    IsolatedStorageFile.GetUserStoreForApplication().CreateDirectory(GetDirectory(target));
                IsolatedStorageFileStream outFile = isf.CreateFile(target);
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

        private static Stream GetFileStream(string filename)
        {
            Uri fileUri = new Uri(filename, UriKind.Relative);
            StreamResourceInfo stream = System.Windows.Application.GetResourceStream(fileUri);

            if (stream != null) {
                return stream.Stream;
            }
            return null;
        }

        public static String GetDirectory(string fileName)
        {
            if (fileName.Contains("/"))
                return fileName.Substring(0, fileName.LastIndexOf('/'));
            else
                return string.Empty;
        }


    }
}
