using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Agent.Model
{
    class SQLiteDriver
    {

        private string dbFileName;
        private SQLiteFactory sqlFactory;
        private SQLiteConnection connection;

        public SQLiteDriver(string dbFileName)
        {
            try
            {
                this.dbFileName = dbFileName;
                if (!File.Exists(this.dbFileName))
                {
                    SQLiteConnection.CreateFile(this.dbFileName);
                }
                this.sqlFactory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
                this.connection = (SQLiteConnection)this.sqlFactory.CreateConnection();
                this.connection.ConnectionString = "Data Source = " +dbFileName;
                this.connection.Open();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        public int NonExecuteQuery(string query)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(this.connection);
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return 0;
            }
        }

        public SQLiteDataReader Query(string query)
        {
            SQLiteCommand command = new SQLiteCommand(this.connection);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            SQLiteDataReader reader = command.ExecuteReader();
            return reader;
        }

        public void Close()
        {
            try
            {
                this.connection.Close();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }
    }
}
