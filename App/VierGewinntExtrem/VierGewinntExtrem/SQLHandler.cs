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
            this.connection_init_string = "datasource=127.0.0.1;port=3306;username=root;password=;database=viergewinnt;";
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
            MySqlCommand dbcommand = this.connection.CreateCommand();
            dbcommand.CommandText = cmd;
            //command.Parameters.Add("@username", txtUserName.Text);
            //command.Parameters.Add("@password", txtPassword.Text);
            dbcommand.ExecuteScalar();

            // this.command = new MySqlCommand(cmd);
            //IDK if that works, it should execute the command
            //and return an int, which is deleted immediatly.

            _ = this.command.ExecuteNonQuery();
        }

        ~SQLHandler()
        {
            //The handler closes the connection when the obj is deleted,
            //no reconnection overhead while runtime.
            this.connection?.Close();
        }
    }
}
