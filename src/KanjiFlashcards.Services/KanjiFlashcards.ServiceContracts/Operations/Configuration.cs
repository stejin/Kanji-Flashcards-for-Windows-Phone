using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace KanjiFlashcards.ServiceContracts.Operations
{
    public class KanjiDatabaseCurrentVersionRequest
    {
    }

    public class KanjiDatabaseCurrentVersionResponse
    {
        public int Version { get; set; }
    }

    public class TestDevPasswordRequest
    {
        public string Password { get; set; }
    }

    public class TestDevPasswordResponse
    {
        public bool IsValid { get; set; }
    }


}