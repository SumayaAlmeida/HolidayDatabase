//////////////////////////////////////////////////////////////////////////////////
// Level 3 Creating an object oriented computer program using C# (7540-039)
//Candidate's name: Sumaya Ferreira de Almeida
// City and Guilds Number: BWK 7540
// Date: 10/05/2021
///////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HolidayDatabase
{
    public partial class FrmHoliday : Form
   {
        DBConnection db = null;

        List<Holiday> holidayList = null;

        int currentIndex = 0;

        bool clickToClear = true;

        int noOfDays = 1;

        //METHODS////////////////////////////////////////////////////////////////////////////////////////////////////
        private void MakeDBConnection()
        {
            this.db = new DBConnection("localhost", "travel", "csharp", "password");

            if (this.db.Connect() == false)
            {
                MessageBox.Show("Problem connecting to MySql Server.");
            }
        }

        private void DisplayHolidaysInfo(int index)
        {
            Holiday holiday = this.holidayList[index];

            this.textBoxHolidayNo.Text = $"{holiday.HolidayNo}";
            this.textBoxDestination.Text = holiday.Destination;
            this.textBoxCost.Text = $"€{holiday.Cost:n2}";
            //this.textBoxDepartureDate.Text = $"{holiday.DepartureDate:yyyy-MM-dd}";
            this.textBoxDepartureDate.Text = $"{holiday.DepartureDate:dd/MM/yyyy}";
            this.textBoxNumberOfDays.Text = $"{holiday.NoOfDays}";
            if (holiday.Available == 1)
            {
                this.checkBoxAvailable.Checked = true;
            }
            else
            {
                this.checkBoxAvailable.Checked = false;
            }

            this.textBoxNavigationStatus.Text = BuildNavigationStatus();
        }

        private string BuildNavigationStatus()
        {
            return $"{this.currentIndex + 1} of {this.holidayList.Count}";
        }
        private void LoadHolidays()
        {
            DBHolidays dbHolidays = new DBHolidays();

            MySqlDataReader reader = dbHolidays.GetHolidays(this.db.Connection);

            while (reader.Read())
            {
                int holidayNo = reader.GetInt32(0);
                string destination = reader.GetString(1);
                double cost = reader.GetDouble(2);
                DateTime departureDate = reader.GetDateTime(3);
                int noOfDays = reader.GetInt32(4);
                int available = reader.GetInt32(5);

                Holiday holiday = new Holiday(holidayNo, destination, cost, departureDate, noOfDays, available);

                // add the holidays to the list of holidays
                this.holidayList.Add(holiday);
            }

            reader.Close();

        }
        private void frmHoliday_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.db != null)
            {
                this.db.Close();
            }
        }

        public void DisableButtons()
        {
            buttonDelete.Enabled = false;
            buttonUpdate.Enabled = false;
            buttonFirst.Enabled = false;
            buttonNext.Enabled = false;
            buttonPrevious.Enabled = false;
            buttonLast.Enabled = false;
            buttonPrint.Enabled = false;
        }

        public void EnableButtons()
        {
            buttonDelete.Enabled = true;
            buttonUpdate.Enabled = true;
            buttonFirst.Enabled = true;
            buttonNext.Enabled = true;
            buttonPrevious.Enabled = true;
            buttonLast.Enabled = true;
            buttonPrint.Enabled = true;
        }

        private string GetHolidayDescriptions()
        {
            string holidayDescriptions = "HolidayNo   Destination            Departure Date      Cost      Available\r\n" +
                                 "----------------------------------------------------------------------------------\r\n"; ;

            foreach (Holiday holiday in this.holidayList)
            {
                holidayDescriptions += holiday.ToString();
                holidayDescriptions += "\n";
            }

            return holidayDescriptions;
        }

        // CREATE NEW HOLIDAY OBJECT FROM USER INPUT - VALIDATE FORMATS AND MAX/MIN REQUIREMENTS///////////////////////////////////

        int holidayNo;
        private Holiday CreateHolidayFromForm()
        {

            try
            {
                int input = Int32.Parse(textBoxHolidayNo.Text);
                if (ValidateHolidayNo(input))
                {
                    holidayNo = input;
                }
                else
                {
                    return null; ;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid HolidayNo. It must be a numeric value between 200 and 1000", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            string destination = this.textBoxDestination.Text;
            if (ValidateDestination(destination) == false)
            {
                return null;
            }

            //HANDLE MULTIPLE WAYS USER MAY INPUT THE COST - INCLUDING USING CURRENCY SYMBOL AND COMMMA TO SEPARATE CENTS/

            string costSubstring = "";
            double cost;
            if (!textBoxCost.Text.Contains(','))
            {
                if (this.textBoxCost.Text.Length > 0 && this.textBoxCost.Text[0] == '€')
                {

                    costSubstring = this.textBoxCost.Text.Substring(1);
                    if (double.TryParse(costSubstring, out cost) == false)
                    {
                        MessageBox.Show("Please enter a valid cost.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return null;
                    }

                }
                else
                {
                    if (double.TryParse(textBoxCost.Text, out cost) == false)
                    {
                        MessageBox.Show("Please enter a valid cost.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return null;
                    }
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid cost. Use dot instead of comma to separate Euros from cents", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }

            // PARSE STRING TO DATE AND CHECK IF INPUT IS A FUTURE DATE

            DateTime departureDate;

            if (DateTime.TryParse(this.textBoxDepartureDate.Text, out departureDate) == false)
            {
                MessageBox.Show("Please enter a date in the format YYYY-MM-DD", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return null;
            }
            else
            {
                if (departureDate < DateTime.Now)
                {
                    MessageBox.Show("Please enter a date in the future", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }
                    
            }

            if (ValidateNumberOfDays(textBoxNumberOfDays.Text))
            {
                try
                {
                    noOfDays = Int32.Parse(textBoxNumberOfDays.Text);

                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter a valid Number of Days. It must be a numeric value greater than 0", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return null;
                }
            }
            // IF NUMBER OF DAYS INPUT IS EMPTY CHECK IF THE USER IS TRYING TO ADD OR UPDATE TO AN EMPTY NO OF DAYS/////
            else
            {
                if (clickToClear) //IF UPDATING NOOFDAYS == NOOFDAYS IT WERE PREVIOUSLY
                {
                    noOfDays = holidayList[currentIndex].NoOfDays;
                    DisplayHolidaysInfo(currentIndex);
                }
                else //IF ADDING A NEW HOLIDAY WITH EMPTY NO OF DAYS NO OF DAYS == 1 
                {
                    noOfDays = 1;
                    DisplayHolidaysInfo(currentIndex);
                }

                
            }
            
            int available = 0;
            if (this.checkBoxAvailable.Enabled)
            {
                available = 1;
            }

            Holiday holiday = new Holiday (holidayNo, destination, cost, departureDate, noOfDays, available);

            return holiday;
        }

        // VALIDATION //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool ValidateHolidayNo(int holidayNo)
        {
            if (holidayNo >= 200 && holidayNo <= 1000)
            {
                return true;
            }

            MessageBox.Show("1: Holiday number must be in range 200 to 1000.", "Information",MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private bool ValidateDestination(string destination)
        {
            if (destination.Length > 0 && destination.Length <= 20)
            {
                return true;
            }

            MessageBox.Show("Destination must be greater than 0 characters, but no more than 20", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
        }

        private bool ValidateNumberOfDays(string stringNumber)
        {
            if (stringNumber.Length > 0)
            {
                return true;
            }
            MessageBox.Show("Number of days cannot be empty", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return false;
            
        }
            
        //EVENT HANDLERS//////////////////////////////////////////////////////////////////////////////////////////////////////

        public FrmHoliday()
        {
            InitializeComponent();
        }
        private void FrmHoliday_Load(object sender, EventArgs e)
        {

            this.Text = "Task A - Sumaya Almeida - " + DateTime.Now.ToShortDateString();

            ToolTip toolTip = new ToolTip();

            toolTip.SetToolTip(this.textBoxDepartureDate, "Enter the date. Format: dd/mm/YYYY.");
            toolTip.SetToolTip(this.textBoxDestination, "Enter the destination - max 20 characters.");
            toolTip.SetToolTip(this.textBoxCost, "Enter the cost per night. Use '.' (dot) to separate cents");

            this.holidayList = new List<Holiday>();

            MakeDBConnection();

            // Gets the records from the travel database
            LoadHolidays();

            // Display the first element in the holidayList to screen
            DisplayHolidaysInfo(currentIndex);
        }

        private void buttonFirst_Click(object sender, EventArgs e)
        {
            this.currentIndex = 0;
            DisplayHolidaysInfo(this.currentIndex);
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (this.currentIndex > 0)
            {
                this.currentIndex--;

                DisplayHolidaysInfo(this.currentIndex);
            }
            else
            {
                MessageBox.Show("There are no earlier records.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (this.currentIndex + 1 < this.holidayList.Count)
            {
                this.currentIndex++;

                DisplayHolidaysInfo(this.currentIndex);
            }
            else
            {
                MessageBox.Show("No more records exist.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void buttonLast_Click(object sender, EventArgs e)
        {
            this.currentIndex = this.holidayList.Count - 1;
            DisplayHolidaysInfo(this.currentIndex);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
           
            DisplayHolidaysInfo(this.currentIndex);
            buttonAdd.Text = "&Add";
            clickToClear = true;
            EnableButtons();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            DBHolidays dBHolidays = new DBHolidays();

            int originalHolidayNo = this.holidayList[this.currentIndex].HolidayNo;

            Holiday holiday = CreateHolidayFromForm();

            if (holiday == null)
            {
                return;
            } 

            if(dBHolidays.Update(this.db.Connection, originalHolidayNo, holiday) == true)
            {
                this.holidayList[this.currentIndex] = holiday;
                MessageBox.Show($"Holiday No.: {holiday.HolidayNo} has been updated", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayHolidaysInfo(this.currentIndex);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (clickToClear)
            {
                textBoxHolidayNo.Text = "";
                textBoxDestination.Text = "";
                textBoxCost.Text = "";
                textBoxDepartureDate.Text = "";
                textBoxNumberOfDays.Text = "";
                checkBoxAvailable.Checked = false;
                buttonAdd.Text = "&Save";
                clickToClear = false;

                DisableButtons();

            }
            else
            {
                DBHolidays dbHolidays = new DBHolidays();
                Holiday holiday = CreateHolidayFromForm();

                if (holiday == null)
                {
                    return;
                }

                // ADD NEW RECORD TO DATABASE

                if (dbHolidays.Insert(this.db.Connection, holiday) == true)
                {
                    this.holidayList.Add(holiday);
                    this.currentIndex = this.holidayList.Count - 1;
                    DisplayHolidaysInfo(this.currentIndex);

                    MessageBox.Show($"Holiday No.: {holiday.HolidayNo} has been added to the database", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    buttonAdd.Text = "&Add";
                    clickToClear = true;
                    EnableButtons();

                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            DBHolidays dbHolidays = new DBHolidays();

            int holidayNo = this.holidayList[this.currentIndex].HolidayNo;

            DialogResult result = MessageBox.Show($"Are you sure you want to delete Holiday No {holidayNo}?", "Verify Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            if (dbHolidays.Delete(this.db.Connection, holidayNo))
            {
                this.holidayList.RemoveAt(this.currentIndex);

                if (this.currentIndex == this.holidayList.Count)
                {
                    this.currentIndex--;
                }

                DisplayHolidaysInfo(this.currentIndex);
            }
        }

         private void buttonPrint_Click(object sender, EventArgs e)
         {

            string holidayDescriptions = GetHolidayDescriptions();

            DocumentPrinter documentPrinter = new DocumentPrinter(holidayDescriptions, "Downtown Travel");

            documentPrinter.Print();

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        
    }
}

