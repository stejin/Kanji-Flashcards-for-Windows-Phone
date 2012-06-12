using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KanjiDatabase
{
    public enum OperationResult
    {
        Success,
        Fail
    }

    public class SaveResponse
    {
        public Int64 SavedObjectKey { get; set; }
        public OperationResult Result { get; set; }

        public SaveResponse(OperationResult operationResult)
        {
            Result = operationResult;
        }

    }
}
