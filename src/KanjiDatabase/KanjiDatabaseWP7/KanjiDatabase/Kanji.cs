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
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;

namespace KanjiDatabase
{
    [Index(Columns = "JLPTLevel", IsUnique = false, Name = "kanji_JLPTLevel")]
    [Table]
    public class Kanji
    {

        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }

        [Column(CanBeNull = false)]
        public string Literal { get; set; }

        [Column]
        public string Meaning { get; set; }

        [Column]
        public string OnYomi { get; set; }

        [Column]
        public string KunYomi { get; set; }

        [Column(CanBeNull = false)]
        public JLPT JLPTLevel { get; set; }

        [Column(CanBeNull = false)]
        public int StrokeCount { get; set; }

    }
}
