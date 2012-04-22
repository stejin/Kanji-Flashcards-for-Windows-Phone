using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using KanjiFlashcards.ServiceContracts.Types;
using KanjiFlashcards.ServiceContracts.Operations;

namespace KanjiFlashcards.DataLayer
{
    public class DataGateway
    {
        public static DataGateway Instance = new DataGateway();

        #region SQL

        private const string SQL_SCHEMAINFO_SELECT = "Select DatabaseVersion, DevPassword From SchemaInfo";

        private const string SQL_KANJI_JLPT_SELECT = "Select Id, Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount From Kanji Where JLPT In ({0}) Order By Id";
        
        private const string SQL_KANJI_ID_SELECT = "Select Id, Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount From Kanji Where Id = {0}";

        private const string SQL_KANJI_ALL_SELECT = "Select Id, Literal, OnYomi, KunYomi, Meaning, JLPT, StrokeCount From Kanji Order By Id";

        #endregion

        private IDatabaseClient OpenDatabaseClient()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["connectionString"];
            switch (connectionString.ProviderName) {
                case "MySql.Data.MySqlClient":
                    return new MySqlClient(connectionString.ConnectionString);
                default:
                    return null;
            }
        }

        #region public

        public int GetKanjiDatabaseVersion()
        {
            int result = 0;
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_SCHEMAINFO_SELECT;
                DataTable data = client.Read();
                if (data.Rows.Count > 0) {
                    var row = data.Rows[0];
                    result = Convert.ToInt32(row["DatabaseVersion"]);
                }
            }
            return result;
        }

        public string GetDevPassword()
        {
            string result = "";
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_SCHEMAINFO_SELECT;
                DataTable data = client.Read();
                if (data.Rows.Count > 0) {
                    var row = data.Rows[0];
                    result = row["DevPassword"].ToString();
                }
            }
            return result;
        }

        public KanjiMessage GetKanjiForToday(KanjiForTodayRequest request)
        {
            var result = new KanjiMessage();
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = String.Format(SQL_KANJI_JLPT_SELECT, GetJlptString(request.JlptLevels));
                DataTable data = client.Read();
                if (data.Rows.Count > 0) {
                    int rowNumber = (DateTime.Today.Year * 365 + DateTime.Today.DayOfYear) % data.Rows.Count;
                    var row = data.Rows[rowNumber];
                    result.Id = Convert.ToInt32(row["Id"]);
                    result.Literal = row["Literal"].ToString();
                    result.OnYomi = row["OnYomi"].ToString();
                    result.KunYomi = row["KunYomi"].ToString();
                    result.Meaning = row["Meaning"].ToString();
                    result.Jlpt = (JLPT)Enum.Parse(typeof(JLPT), row["JLPT"].ToString());
                    result.StrokeCount = Convert.ToInt32(row["StrokeCount"]);
                }
            }
            return result;
        }

        public KanjiMessage GetKanjiForId(KanjiForIdRequest request)
        {
            var result = new KanjiMessage();
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = String.Format(SQL_KANJI_ID_SELECT, request.Id.ToString());
                DataTable data = client.Read();
                if (data.Rows.Count > 0) {
                    var row = data.Rows[0];
                    result.Id = Convert.ToInt32(row["Id"]);
                    result.Literal = row["Literal"].ToString();
                    result.OnYomi = row["OnYomi"].ToString();
                    result.KunYomi = row["KunYomi"].ToString();
                    result.Meaning = row["Meaning"].ToString();
                    result.Jlpt = (JLPT)Enum.Parse(typeof(JLPT), row["JLPT"].ToString());
                    result.StrokeCount = Convert.ToInt32(row["StrokeCount"]);
                } else {
                    result.Id = 0;
                    result.Jlpt = JLPT.Undefined;
                }
            }
            return result;
        }

        public List<KanjiMessage> GetAllKanji()
        {
            var result = new List<KanjiMessage>();
            using (IDatabaseClient client = OpenDatabaseClient()) {
                client.SelectCommand = SQL_KANJI_ALL_SELECT;
                DataTable data = client.Read();
                for (int i = 0; i < data.Rows.Count; i++) {
                    var row = data.Rows[i];
                    var kanji = new KanjiMessage();
                    kanji.Id = Convert.ToInt32(row["Id"]);
                    kanji.Literal = row["Literal"].ToString();
                    kanji.OnYomi = row["OnYomi"].ToString();
                    kanji.KunYomi = row["KunYomi"].ToString();
                    kanji.Meaning = row["Meaning"].ToString();
                    kanji.Jlpt = (JLPT)Enum.Parse(typeof(JLPT), row["JLPT"].ToString());
                    kanji.StrokeCount = Convert.ToInt32(row["StrokeCount"]);
                    result.Add(kanji);
                }
            }
            return result;
        }

        #endregion

        # region private

        private string GetJlptString(JLPT jlptLevels)
        {
            var result = new StringBuilder();

            if ((jlptLevels & JLPT.Level1) == JLPT.Level1)
                result.Append("'Level1',");

            if ((jlptLevels & JLPT.Level2) == JLPT.Level2)
                result.Append("'Level2',");

            if ((jlptLevels & JLPT.Level3) == JLPT.Level3)
                result.Append("'Level3',");

            if ((jlptLevels & JLPT.Level4) == JLPT.Level4)
                result.Append("'Level4',");

            if ((jlptLevels & JLPT.Other) == JLPT.Other)
                result.Append("'Other',");

            if (result.Length == 0)
                return "''";
            else
                return result.Remove(result.Length - 1, 1).ToString();
        }

        #endregion

    }
}
