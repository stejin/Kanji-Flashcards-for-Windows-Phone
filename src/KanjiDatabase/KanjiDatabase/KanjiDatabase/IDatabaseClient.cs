using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace KanjiDatabase
{
    public interface IDatabaseClient : IDisposable
    {
        string SelectCommand { get; set; }
        string UpdateCommand { get; set; }
        string InsertCommand { get; set; }
        string DeleteCommand { get; set; }
        void AddCommandParameter(string name, object value);
        void ClearCommandParameters();
        DataTable Read();
        int Update(DataTable data);
        Int64 InsertRow(DataRow data, string idColumnName);
    }
}
