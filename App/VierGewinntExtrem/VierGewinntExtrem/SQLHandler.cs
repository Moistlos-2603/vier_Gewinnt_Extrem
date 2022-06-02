using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;

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
        /// Execute SQL Insert Commands
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

        }


        /// <summary>
        /// Execute SQL Select requests and returns a List of a Lists of strings
        /// </summary>
        public List<List<string>> Query(string cmd)
        {

            var data = new List<List<string>>();
            using (var command = this.connection.CreateCommand())
            {
                command.CommandText = cmd;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data.Add(new List<string>());
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            data[data.Count - 1].Add(reader.GetString(i));
                        }

                    }
                }
            }
            return data;
        }

        /// <summary>
        /// Converts a List of Lists of strings (List<List<string>>) and converts it into a list of strings, where every string resembles one entry
        /// </summary>
        public List<string> DataToString(List<List<string>> idk)
        {
            List<string> ausgabe = new List<string>();
            int counter = 0;
            foreach (List<string> id in idk)
            {
                ausgabe.Add("");
                foreach (string s in id)
                {
                    ausgabe[counter] += s + ", ";
                }
                counter++;
            }
            return ausgabe;
        }

        ~SQLHandler()
        {
            //The handler closes the connection when the obj is deleted,
            //no reconnection overhead while runtime.
            this.connection?.Close();
        }
    }
}
