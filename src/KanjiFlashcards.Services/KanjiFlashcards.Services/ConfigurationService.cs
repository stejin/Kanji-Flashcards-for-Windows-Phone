using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KanjiFlashcards.DataLayer;
using KanjiFlashcards.ServiceContracts.Operations;

namespace KanjiFlashcards.Services
{
    public class ConfigurationService 
    {
        public KanjiDatabaseCurrentVersionResponse Execute(KanjiDatabaseCurrentVersionRequest request)
        {
            return new KanjiDatabaseCurrentVersionResponse() { Version = DataGateway.Instance.GetKanjiDatabaseVersion() };
        }

        public TestDevPasswordResponse Execute(TestDevPasswordRequest request)
        {
            return new TestDevPasswordResponse() { IsValid = (request.Password == DataGateway.Instance.GetDevPassword()) };
        }
    }
    
}