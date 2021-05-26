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
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HolidayDatabase
{
    class DBHolidays
    {

        public MySqlDataReader GetHolidays(MySqlConnection connection)
        {
            string sql = "SELECT * FROM tblholiday;";

            MySqlCommand cmd = new MySqlCommand(sql, connection);

            MySqlDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
            }

            return reader;
        }

        public bool Update(MySqlConnection connection, int originalHolidayNo, Holiday holiday)
        {

            string sql = "UPDATE tblholiday SET HolidayNo = @holidayNo, Destination = @destination, Cost = @cost, DepartureDate = @departureDate, NoOfDays = @numberOfDays, Available = @available WHERE HolidayNo = @originalHolidayNo;";

            MySqlCommand cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@originalHolidayNo", originalHolidayNo);
            cmd.Parameters.AddWithValue("@holidayNo", holiday.HolidayNo);
            cmd.Parameters.AddWithValue("@destination", holiday.Destination);
            cmd.Parameters.AddWithValue("@cost", holiday.Cost);
            cmd.Parameters.AddWithValue("@departureDate", holiday.DepartureDate);
            cmd.Parameters.AddWithValue("@numberOfDays", holiday.NoOfDays);
            cmd.Parameters.AddWithValue("@available", holiday.Available);

            try
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
                return false;
            }

            return true;
        }

        public bool Insert(MySqlConnection connection, Holiday holiday)
        {
            string sql = "INSERT INTO tblholiday VALUES(@holidayNo, @destination, @cost, @departureDate, @numberOfDays, @available);";

            MySqlCommand cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@holidayNo", holiday.HolidayNo);
            cmd.Parameters.AddWithValue("@destination", holiday.Destination);
            cmd.Parameters.AddWithValue("@cost", holiday.Cost);
            cmd.Parameters.AddWithValue("@departureDate", holiday.DepartureDate);
            cmd.Parameters.AddWithValue("@numberOfDays", holiday.NoOfDays);
            cmd.Parameters.AddWithValue("@available", holiday.Available);

            try
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
                return false;
            }

            return true;
        }

        public bool Delete(MySqlConnection connection, int holidayNo)
        {
            string sql = "DELETE FROM tblholiday WHERE HolidayNo = @holidayNo;";

            MySqlCommand cmd = new MySqlCommand(sql, connection);

            cmd.Parameters.AddWithValue("@holidayNo", holidayNo);

            try
            {
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Exception found: " + ex.Message);
                return false;
            }

            return true;
        }


    }
}
