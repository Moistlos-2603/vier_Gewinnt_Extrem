using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;

namespace VierGewinntExtrem
{
    internal class SQLHandler
    {
        /// <summary>
        /// This class handles the connection automatically and provides an
        /// very ligntweight interface for the SQL command execution.
        /// </summary>
        
        private string connection_init_string;
        private MySqlConnection connection;
        //private MySqlCommand? command;
        private DataSet? data_set;
        private DataTable data_table;
        private MySqlDataAdapter adapter;

        public SQLHandler()
        {
            //connection stuff.
            this.connection_init_string = "datasource=127.0.0.1;port=3306;username=root;password=;database=viergewinnt;";
            this.connection = new MySqlConnection(this.connection_init_string);
            try
            {
                // Open the database.
                this.connection.Open();
                data_set = new DataSet();


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
            MySqlCommand dbcommand = connection.CreateCommand();
            dbcommand.CommandText = cmd;
            //command.Parameters.Add("@username", txtUserName.Text);
            //command.Parameters.Add("@password", txtPassword.Text);
            
            dbcommand.ExecuteScalar();

            //I am not sure, but as I understand the adapter is for the data set and data table object
            //very interesting since it's filling them with Fill().
            //I think it has to be linked after the command was run.
            adapter = new MySqlDataAdapter(dbcommand);
        }


        /// <summary>
        /// Execute SQL Select requests and returns a List of a Lists of strings
        /// </summary>
        public List<List<string>> Query(string cmd)
        {
            /*
             * Creates an command object and sets the text to the given argument.
             * Executes sayed command and iterates through the output to append everything to the data list.
             * Returns the data list.
             */
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

                //I am not sure, but as I understand the adapter is for the data set and data table object
                //very interesting since it's filling them with Fill().
                //I think it has to be linked after the command was run.
                adapter = new MySqlDataAdapter(command);
            }
            return data;
        }

        /// <summary>
        /// Converts a List of Lists of strings (List<List<string>>) and converts it into a list of strings, where every string resembles one entry
        /// </summary>
        public List<string> DataToString(List<List<string>> idk)
        {
            /*
             * Iterates through the as Data defined input and concats it to an continous string.
             * The concated data is seperated by an ','.
             * Returns an list of those columns.
             */
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

        //Removes object when the scope is closed, in wich it was defined before.
        ~SQLHandler()
        {
            //The handler closes the connection when the obj is deleted,
            //no reconnection overhead while runtime.
            this.connection?.Close();
        }

        //Return the data set.
        public DataSet DataSet
        {
            get
            {
                if(adapter != null && data_set != null)
                {
                    adapter.Fill(data_set);
                }
                return data_set;
            }
        }

        public DataTable DataTable
        {
            get
            {
                if(adapter != null)
                {
                    adapter.Fill(data_table);
                }
                return data_table;
            }
        }
    }
}
