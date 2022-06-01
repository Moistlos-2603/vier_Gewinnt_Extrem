using MySql.Data.MySqlClient;
using System.Data;

namespace VierGewinntExtrem
{
    internal class SQLHandler
    {
        /// <summary>
        /// I don't know if it works. Requieres probably work on it.
        /// </summary>
        private string connection_init_string;
        //string query = "SELECT * FROM lehrer";
        private MySqlConnection? connection;
        private MySqlCommand? command;
        private DataSet? data_set;

        public SQLHandler()
        {
            //connection stuff.
            this.connection_init_string = "datasource=127.0.0.1;port=3306;username=root;password=;database=VierGewinntLiga;";
            this.connection = new MySqlConnection(this.connection_init_string);
            try
            {
                // Open the database
                this.connection.Open();
                data_set = new DataSet();

                // Finally close the connection*/
                
            }
            catch (System.Exception ex)
            {
                //No warning!
                _ = ex;
                // Silent error, dangerous!
            }
        }

        /// <summary>
        /// This maybe doesn't work!
        /// </summary>
        /// <param name="cmd"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Execute(string cmd)
        {
            this.command = new MySqlCommand(cmd);
            //IDK if that works, it should execute the command
            //and return an int, which is deleted immediatly.
            _ = command.ExecuteNonQuery();
        }

        ~SQLHandler()
        {
            //The handler closes the connection when the obj is deleted,
            //no reconnection overhead while runtime.
            this.connection?.Close();
        }
    }
}
