//////////////////////////////////////////////////////////////////////////////////
// Level 3 Creating an object oriented computer program using C# (7540-039)
//Candidate's name: Sumaya Ferreira de Almeida
// City and Guilds Number: BWK 7540
// Date: 10/05/2021
///////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace HolidayDatabase
{
    class DBConnection
    {
        // Store the MySql connection here for use later
        MySqlConnection connection = null;

        string connectionString;

        public MySqlConnection Connection { get => connection; }

        // Connection string will be composed of the following information
        //   server name
        //   database
        //   userID
        //   password

        public DBConnection(string serverName, string databaseName, string userID, string password)
        {
            // Format connection string

            // Example:
            // SERVER=localhost;DATABASE=travel;UID=csharp;PASSWORD=password;

            this.connectionString = $"SERVER={serverName};DATABASE={databaseName};UID={userID};PASSWORD={password};";

        }

        public DBConnection(MySqlConnection connection, string connectionString)
        {
            this.connection = connection;
            this.connectionString = connectionString;
        }

        public bool Connect()
        {
            bool succeeded = true;

            try
            {
                this.connection = new MySqlConnection(this.connectionString);

                this.connection.Open();
            }
            catch (MySqlException ex)
            {
                succeeded = false;

                switch(ex.Number)
                {
                    case 0:
                        MessageBox.Show("Authentication error - please check your login credentials.");
                        break;

                    case 1045:
                        MessageBox.Show("Cannot connect to server.");
                        break;

                    default:
                        MessageBox.Show("Exception found: " + ex.Message);
                        break;
                }
            }


            return succeeded;
        }

        public bool Close()
        {
            bool succeeded = true;

            try
            {
                this.connection.Close();
            }
            catch(MySqlException ex)
            {
                succeeded = false;
                MessageBox.Show("Exception found: " + ex.Message);
            }

            return succeeded;
        }
    }
}
