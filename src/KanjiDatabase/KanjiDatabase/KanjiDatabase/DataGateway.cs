using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using KanjiDatabase;

namespace KanjiDatabase
{
    public class DataGateway
    {
        public static DataGateway Instance = new DataGateway();

        #region SQL

        private const string SQL_KANJI_ALL_SELECT = "Select Id, Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount From Kanji";

        private const string SQL_KANJI_ID_SELECT = "Select Id, Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount From Kanji Where Id = @Id";

        private const string SQL_KANJI_LITERAL_SELECT = "Select Id, Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount From Kanji Where Literal = @Literal";

        private const string SQL_KANJI_UPDATE = "Update Kanji Set Literal = @Literal, OnYomi = @OnYomi, KunYomi = @KunYomi, Meaning = @Meaning, JLPT = @JLPT, StrokeCount = @StrokeCount Where Id = @Id";

        private const string SQL_KANJI_INSERT = "Insert Into Kanji (Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount) Values (@Literal, @OnYomi, @KunYomi, @Meaning, @JLPT, @StrokeCount)";
        
        private const string SQL_KANJI_DELETE = "Delete From Kanji Where Id = @Id";

        #endregion

        private IDatabaseClient OpenDatabaseClient()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["connectionString"];
            switch (connectionString.ProviderName) {
                case "MySql.Data.MySqlClient":
                    return new MySqlClient(connectionString.ConnectionString);
                case "System.Data.SqlClient":
                    return new SqlServerClient(connectionString.ConnectionString);
                default:
                    return null;
            }
        }

        #region Kanji

        public List<KanjiData> GetAllKanji()
        {
            var result = new List<KanjiData>();
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_KANJI_ALL_SELECT;
                DataTable data = client.Read();
                foreach (DataRow row in data.Rows)
                    result.Add(new KanjiData() { Id = Convert.ToInt32(row["Id"]), Literal = row["Literal"].ToString(), OnYomi = row["OnYomi"].ToString(), KunYomi = row["KunYomi"].ToString(), Meaning = row["Meaning"].ToString(), JLPTLevel = (JLPT)Enum.Parse(typeof(JLPT), row["JLPT"].ToString()), StrokeCount = Convert.ToInt32(row["StrokeCount"]) });
            }
            return result;
        }

        public KanjiData GetKanjiForId(int id)
        {
            KanjiData result = null;
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_KANJI_ID_SELECT;
                client.AddCommandParameter("Id", id);
                DataTable data = client.Read();
                if (data.Rows.Count > 0) {
                    var row = data.Rows[0];
                    result = new KanjiData() { Literal = row["Literal"].ToString(), OnYomi = row["OnYomi"].ToString(), KunYomi = row["KunYomi"].ToString(), Meaning = row["Meaning"].ToString(), JLPTLevel = (JLPT)Enum.Parse(typeof(JLPT), row["JLPT"].ToString()), StrokeCount = Convert.ToInt32(row["StrokeCount"]) };
                }
            }
            return result;
        }

        public KanjiData GetKanjiForLiteral(string literal)
        {
            KanjiData result = null;
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_KANJI_ID_SELECT;
                client.AddCommandParameter("Literal", literal);
                DataTable data = client.Read();
                if (data.Rows.Count > 0) {
                    var row = data.Rows[0];
                    result = new KanjiData() { Literal = row["Literal"].ToString(), OnYomi = row["OnYomi"].ToString(), KunYomi = row["KunYomi"].ToString(), Meaning = row["Meaning"].ToString(), JLPTLevel = (JLPT)Enum.Parse(typeof(JLPT), row["JLPT"].ToString()), StrokeCount = Convert.ToInt32(row["StrokeCount"]) };
                }
            }
            return result;
        }

        public SaveResponse InsertKanji(KanjiData kanji)
        {
            using (IDatabaseClient client = OpenDatabaseClient())
            {
                client.SelectCommand = SQL_KANJI_ID_SELECT;
                client.InsertCommand = SQL_KANJI_INSERT;
                          
                DataTable data;

                client.AddCommandParameter("Id", 0);
                data = client.Read();
                client.ClearCommandParameters();

                DataRow newRow = data.NewRow();
                // newRow["Id"] = key;
                newRow["Literal"] = kanji.Literal;
                newRow["OnYomi"] = kanji.OnYomi;
                newRow["KunYomi"] = kanji.KunYomi;
                newRow["Meaning"] = kanji.Meaning;
                newRow["JLPT"] = kanji.JLPTLevel.ToString();
                newRow["StrokeCount"] = kanji.StrokeCount;
             
                return new SaveResponse(OperationResult.Success) { SavedObjectKey = client.InsertRow(newRow, "Id") };
            }
        }

        public SaveResponse UpdateKanji(KanjiData kanji)
        {
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_KANJI_LITERAL_SELECT;
                client.UpdateCommand = SQL_KANJI_UPDATE;
                
                client.AddCommandParameter("Literal", kanji.Literal);

                DataTable data;
                data = client.Read();

                data.Rows[0]["OnYomi"] = kanji.OnYomi;
                data.Rows[0]["KunYomi"] = kanji.KunYomi;
                data.Rows[0]["Meaning"] = kanji.Meaning;
                data.Rows[0]["JLPT"] = kanji.JLPTLevel.ToString();
                data.Rows[0]["StrokeCount"] = kanji.StrokeCount;

                client.ClearCommandParameters();
                client.AddCommandParameter("Id", data.Rows[0]["Id"]);
              
                client.Update(data);
                return new SaveResponse(OperationResult.Success);
            }
        }

        public void DeleteKanji(KanjiData kanji)
        {
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_KANJI_LITERAL_SELECT;
                client.DeleteCommand = SQL_KANJI_DELETE;

                client.AddCommandParameter("Literal", kanji.Literal);
                
                DataTable data = client.Read();
                data.Rows[0].Delete();         
               
                client.Update(data);
            }
        }

        #endregion
    }
}
