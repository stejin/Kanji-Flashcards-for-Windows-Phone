using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace KanjiDatabase
{
    public class MySqlClient : IDatabaseClient
    {
        private MySqlConnection conn;
        private MySqlDataAdapter adapter;

        public string SelectCommand { get; set; }
        public string UpdateCommand { get; set; }
        public string InsertCommand { get; set; }
        public string DeleteCommand { get; set; }

        private List<MySqlParameter> CommandParameters { get; set; }

        public MySqlClient(string connectionString)
        {
            conn = new MySqlConnection(connectionString);
            conn.Open();
            
            adapter = new MySqlDataAdapter();

            SelectCommand = "";
            UpdateCommand = "";
            InsertCommand = "";
            DeleteCommand = "";

            CommandParameters = new List<MySqlParameter>();
        }

                
        public void AddCommandParameter(string name, object value)
        {
            CommandParameters.Add(new MySqlParameter('@' + name, value));
        }

        public void ClearCommandParameters()
        {
            CommandParameters.Clear();
        }

        public DataTable Read()
        {
            DataTable result = new DataTable();
            adapter.SelectCommand = GetSelectCommand();
            adapter.Fill(result);
            return result;
        }

        public int Update(DataTable data)
        {
            int result = 0;
            if (data.GetChanges() != null) {
                adapter.UpdateCommand = GetUpdateCommand();
                adapter.InsertCommand = GetInsertCommand();
                adapter.DeleteCommand = GetDeleteCommand();
                result = adapter.Update(data.GetChanges());
                data.AcceptChanges();
            }
            return result;
        }

        public Int64 InsertRow(DataRow row, string idColumnName)
        {
            Int64 result = 0;
            if (row != null) {
                row.Table.Rows.Add(row);
                adapter.InsertCommand = GetInsertCommand(row);
                adapter.Update(row.Table);
                row.Table.AcceptChanges();
                result = (Int64)adapter.InsertCommand.LastInsertedId;
            }
            return result;
        }

        private MySqlCommand GetSelectCommand()
        {
            return GetCommand(SelectCommand);
        }

        private MySqlCommand GetUpdateCommand()
        {
            return GetCommand(UpdateCommand);
        }

        private MySqlCommand GetInsertCommand()
        {
            return GetCommand(InsertCommand);
        }

        private MySqlCommand GetInsertCommand(DataRow row)
        {
            MySqlCommand insertCommand = GetCommand(row, InsertCommand);
            return insertCommand;
        }

        private MySqlCommand GetDeleteCommand()
        {
            return GetCommand(DeleteCommand);
        }

        private MySqlCommand GetCommand(DataRow row, string sqlCommand)
        {
            MySqlCommand result = new MySqlCommand(sqlCommand, conn);
            for (int i = 0; i < CommandParameters.Count; i++) {
                result.Parameters.Add(new MySqlParameter(CommandParameters[i].ParameterName, CommandParameters[i].Value));
            }
            List<string> mappedParameters = GetParametersFromSql(sqlCommand);
            foreach (string mappedParameter in mappedParameters) {
                if (!result.Parameters.Contains(mappedParameter)) {
                    result.Parameters.AddWithValue(mappedParameter, row[mappedParameter.Replace("@","")]);
                }
            }
            return result;
        }

        private MySqlCommand GetCommand(string sqlCommand)
        {
            MySqlCommand result = new MySqlCommand(sqlCommand, conn);
            for (int i = 0; i < CommandParameters.Count; i++) {
                result.Parameters.Add(new MySqlParameter(CommandParameters[i].ParameterName, CommandParameters[i].Value));
            }
            List<string> mappedParameters = GetParametersFromSql(sqlCommand);
            foreach (string mappedParameter in mappedParameters) {
                if (!result.Parameters.Contains(mappedParameter)) {
                    MySqlParameter newParameter = new MySqlParameter();
                    newParameter.ParameterName = mappedParameter;
                    newParameter.SourceColumn = mappedParameter.Replace("@","");
                    newParameter.Direction = ParameterDirection.Input;
                    result.Parameters.Add(newParameter);
                }
            }
            return result;
        }

        private List<string> GetParametersFromSql(string sqlCommand)
        {
            List<string> result = new List<string>();
            if (sqlCommand.Contains('@')) {
                List<string> tokens = sqlCommand.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>().ConvertAll<string>(paramName => paramName.Trim());
                foreach (string token in tokens) {
                    string newtoken = token.Replace("(", "").Replace(")", "").Replace(",", "");
                    if (newtoken.StartsWith("@"))
                        result.Add(newtoken);
                }
            }
            return result;
        }


        void IDisposable.Dispose()
        {
            if (conn != null) {
                conn.Close();
                conn = null;
            }
            adapter = null;            
        }
    }
}
