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
using NeoDatis.Odb;
using System.IO.IsolatedStorage;
using System.IO;
using NeoDatis.Odb.Core.Query;
using NeoDatis.Odb.Impl.Core.Query.Criteria;
using NeoDatis.Odb.Core.Query.Criteria;
using KanjiDatabase;

namespace Test
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void PageTitle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {

            Helper.CopyBinaryFile(IsolatedStorageFile.GetUserStoreForApplication(), "Data/kanjidic2.neodatis", "Data/kanjidic2.neodatis", true);

            ODB odb = ODBFactory.Open("Data/kanjidic2.neodatis");

            IQuery query = new CriteriaQuery(typeof(KanjiData), Where.Equal("Literal", "愛"));

            var data = odb.GetObjects<KanjiDatabase.KanjiData>(query);

       //     var data = odb.GetObjects<TestClass>();

//            TestClass test = new TestClass();
 //           test.Test = "Hello";

   //         odb.Store(test);

            odb.Close();
        }
       
    }
}