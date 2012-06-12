using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NeoDatis.Odb;
using KanjiDatabase;
using System.Data;
using System.Data.Common;

namespace CleanKanjiDatabase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            ODB odb = ODBFactory.Open(@"C:\Users\Steffen\Documents\Projects\KanjiDatabase\CleanKanjiDatabase\CleanKanjiDatabase\Data\kanjidic2.neodatis");
            var kanjiList = odb.GetObjects<KanjiData>();
            DataTable table = new DataTable();
            table.Columns.Add("IsChecked", typeof(bool));
            table.Columns.Add("Kanji", typeof(string));
            foreach (var kanji in kanjiList) {
                var row = table.NewRow();
                row["IsChecked"] = false;
                row["Kanji"] = String.Format("{0} Reading: {1} / {2} Meaning: {3} JLPT: {4}", kanji.Literal, kanji.OnYomi, kanji.KunYomi, kanji.Meaning, kanji.JLPTLevel.ToString());
                table.Rows.Add(row);
            }
            var view = new DataView(table);
            kanjiGrid.ItemsSource = view;
        }
     
    }
}
            