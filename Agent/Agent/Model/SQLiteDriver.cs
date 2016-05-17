using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace Agent.Model
{
    class SQLiteDriver
    {

        private string dbFileName="Agent.db3";
        private SQLiteFactory sqlFactory;
        private SQLiteConnection connection;

        public SQLiteDriver()
        {
            try
            {
                bool dbcreate = false;
                if (!File.Exists(this.dbFileName))
                {
                    dbcreate = true;
                    SQLiteConnection.CreateFile(this.dbFileName);
                }
                this.sqlFactory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
                this.connection = (SQLiteConnection)this.sqlFactory.CreateConnection();
                this.connection.ConnectionString = "Data Source = " +dbFileName;
                this.connection.Open();
                if (dbcreate)
                {
                    NonExecuteQuery("CREATE TABLE 'iplist' ( 'ip' TEXT NOT NULL UNIQUE )");
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
        }

        private int NonExecuteQuery(string query)
        {
            try
            {
                SQLiteCommand command = new SQLiteCommand(this.connection);
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log.Write(ex);
                return 0;
            }
        }

        private SQLiteDataReader Query(string query)
        {
            SQLiteCommand command = new SQLiteCommand(this.connection);
            command.CommandType = CommandType.Text;
            command.CommandText = query;
            try
            {
                return command.ExecuteReader();
            }
            catch(Exception ex)
            {
                Log.Write(ex);
                NonExecuteQuery("CREATE TABLE 'iplist' ( 'ip' TEXT NOT NULL UNIQUE )");
                return command.ExecuteReader();
            }
        }

        public void AddIP(string IP)
        {
            NonExecuteQuery("INSERT INTO 'iplist' VALUES ('"+IP+"')");
        }
        public List<string> GetAllIP()
        {
            List<string> ipList = new List<string>();
            SQLiteDataReader dr = Query("SELECT * FROM iplist");
            while (dr.Read())
            {
                ipList.Add(dr.GetString(0));
            }
            return ipList;
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
