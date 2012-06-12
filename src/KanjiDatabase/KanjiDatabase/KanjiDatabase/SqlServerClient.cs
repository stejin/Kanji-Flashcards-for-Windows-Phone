using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace KanjiDatabase
{
    public class SqlServerClient : IDatabaseClient
    {
        private SqlConnection conn;
        private SqlDataAdapter adapter;
        
        public string SelectCommand { get; set; }
        public string UpdateCommand { get; set; }
        public string InsertCommand { get; set; }
        public string DeleteCommand { get; set; }

        private List<SqlParameter> CommandParameters { get; set; }
        
        public SqlServerClient(string connectionString)
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
            
            adapter = new SqlDataAdapter();

            SelectCommand = "";
            UpdateCommand = "";
            InsertCommand = "";
            DeleteCommand = "";

            CommandParameters = new List<SqlParameter>();
        }

                
        #region IDisposable Members

        public void Dispose()
        {
            if (conn != null)
                conn.Close();
        }

        #endregion

        #region IDatabaseClient Members

        public void AddCommandParameter(string name, object value)
        {
            CommandParameters.Add(new SqlParameter('@' + name, value));
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
                adapter.InsertCommand.CommandText += " Set @Identity = SCOPE_IDENTITY()";
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@Identity", SqlDbType.BigInt, 0, idColumnName);
                parameter.Direction = ParameterDirection.Output;
                adapter.Update(row.Table);
                row.Table.AcceptChanges();
                result = (Int64)row[idColumnName];
            }
            return result;
        }

        #endregion

        private SqlCommand GetSelectCommand()
        {
            return GetCommand(SelectCommand);
        }

        private SqlCommand GetUpdateCommand()
        {
            return GetCommand(UpdateCommand);
        }

        private SqlCommand GetInsertCommand()
        {
            return GetCommand(InsertCommand);
        }

        private SqlCommand GetInsertCommand(DataRow row)
        {
            SqlCommand insertCommand = GetCommand(row, InsertCommand);
            return insertCommand;
        }

        private SqlCommand GetDeleteCommand()
        {
            return GetCommand(DeleteCommand);
        }

        private SqlCommand GetCommand(DataRow row, string sqlCommand)
        {
            SqlCommand result = new SqlCommand(sqlCommand, conn);
            for (int i = 0; i < CommandParameters.Count; i++) {
                result.Parameters.Add(new SqlParameter(CommandParameters[i].ParameterName, CommandParameters[i].Value));
            }
            List<string> mappedParameters = GetParametersFromSql(sqlCommand);
            foreach (string mappedParameter in mappedParameters) {
                if (!result.Parameters.Contains(mappedParameter)) {
                    result.Parameters.AddWithValue(mappedParameter, row[mappedParameter.Replace("@","")]);
                }
            }
            return result;
        }

        private SqlCommand GetCommand(string sqlCommand)
        {
            SqlCommand result = new SqlCommand(sqlCommand, conn);
            for (int i = 0; i < CommandParameters.Count; i++) {
                result.Parameters.Add(new SqlParameter(CommandParameters[i].ParameterName, CommandParameters[i].Value));
            }
            List<string> mappedParameters = GetParametersFromSql(sqlCommand);
            foreach (string mappedParameter in mappedParameters) {
                if (!result.Parameters.Contains(mappedParameter)) {
                    SqlParameter newParameter = new SqlParameter();
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
        
    }
}
