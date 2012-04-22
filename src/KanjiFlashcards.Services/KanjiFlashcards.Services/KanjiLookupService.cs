using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KanjiFlashcards.DataLayer;
using KanjiFlashcards.ServiceContracts.Operations;

namespace KanjiFlashcards.Services
{
    public class KanjiLookupService
    {

        public KanjiForTodayResponse Execute(KanjiForTodayRequest request)
        {
            var kanjiRequest = new KanjiForTodayRequest() { JlptLevels = request.JlptLevels };
            return new KanjiForTodayResponse() { Kanji = DataGateway.Instance.GetKanjiForToday(kanjiRequest) };
        }

        public KanjiForIdResponse Execute(KanjiForIdRequest request)
        {
            return new KanjiForIdResponse() { Kanji = DataGateway.Instance.GetKanjiForId(request) };
        }
    }
    
}