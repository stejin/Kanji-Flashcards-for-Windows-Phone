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

namespace GenerateKanjiDatabase
{
    public partial class MainPage : PhoneApplicationPage
    {

        private Database db;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            db = new Database();

            db.DatabaseUpdateCompleted += CopyDatabase;
            db.UpdateKanjiDatatabaseFromInternet();
        }

        private void CopyDatabase(object sender, EventArgs e)
        {
            db.CopyData();
            MessageBox.Show("Database copy complete.");
        }
    }
}