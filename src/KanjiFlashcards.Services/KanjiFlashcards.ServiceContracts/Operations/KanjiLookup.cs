using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KanjiFlashcards.ServiceContracts.Types;

namespace KanjiFlashcards.ServiceContracts.Operations
{
    public class KanjiForTodayRequest
    {
        public JLPT JlptLevels { get; set; }
    }

    public class KanjiForTodayResponse
    {
        public KanjiMessage Kanji { get; set; }
    }

    public class KanjiForIdRequest
    {
        public int Id { get; set; }
    }

    public class KanjiForIdResponse
    {
        public KanjiMessage Kanji { get; set; }
    }

}
