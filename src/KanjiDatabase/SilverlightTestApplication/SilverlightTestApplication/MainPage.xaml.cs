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


namespace SilverlightTestApplication
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PopulateGrid()
        {
            var util = new XMLUtility();
            util.LoadFromFile();
            try {
                var kanjiList = util.GetAllKanji();
                kanjiGrid.ItemsSource = kanjiList;
                kanjiGrid.Columns.Add(new DataGridTextColumn() { Binding = new System.Windows.Data.Binding("Literal")});
                kanjiGrid.Columns.Add(new DataGridTextColumn() { Binding = new System.Windows.Data.Binding("OnYomi") });
                kanjiGrid.Columns.Add(new DataGridTextColumn() { Binding = new System.Windows.Data.Binding("KunYomi") });
                kanjiGrid.Columns.Add(new DataGridTextColumn() { Binding = new System.Windows.Data.Binding("Meaning") });
            } catch (Exception ex) {
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var clazz = System.Type.GetType("KanjiDatabase.KanjiData,KanjiDatabase, Version=1.0.0.0");
            PopulateGrid();
        }

    
    }
}
