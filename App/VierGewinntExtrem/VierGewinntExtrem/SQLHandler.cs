using System;
using System.Data.SqlClient;
using System.Data;

namespace VierGewinntExtrem
{
    internal class SQLHandler
    {
#if DEBUG
        const string drive = "E:";
#else
    const string drive = "C:";
#endif
        SqlCommand command;
        SqlConnection connection;

        public SQLHandler()
        {
            connection = new("Server=localhost;Integrated security=SSPI;database=master");
            string filedata = "CREATE DATABASE MyDatabase ON PRIMARY " +
                              "(NAME = MyDatabase_Data, " +
                              $"FILENAME = '{drive}:\\MyDatabaseData.mdf', " +
                              "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%)" +
                              "LOG ON (NAME = MyDatabase_Log, " +
                              $"FILENAME = '{drive}:\\MyDatabaseLog.ldf', " +
                              "SIZE = 1MB, " +
                              "MAXSIZE = 5MB, " +
                              "FILEGROWTH = 10%)";

            command = new(filedata, connection);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {

            }
        }

        /// <summary>
        /// This maybe doesn't work!
        /// </summary>
        /// <param name="cmd"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(string cmd)
        {
            command.CommandText = cmd;
            command.ExecuteNonQuery();
            throw new NotImplementedException();
        }

        ~SQLHandler()
        {
            command.Dispose();
            connection?.Close();
        }
    }
}
