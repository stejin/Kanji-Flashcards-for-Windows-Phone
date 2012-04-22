using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace KanjiFlashcards.ServiceContracts.Types
{
    public class KanjiMessage
    {
        
        public int Id { get; set; }

        public string Literal { get; set; }

        public string OnYomi { get; set; }

        public string KunYomi { get; set; }

        public string Meaning { get; set; }

        public JLPT Jlpt { get; set; }

        public int StrokeCount { get; set; }
    }
}