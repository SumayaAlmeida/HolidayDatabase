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

namespace HolidayDatabase
{
    class Holiday
    {
        int holidayNo;
        string destination;
        double cost;
        DateTime departureDate;
        int noOfDays;
        int available;


        public Holiday(int holidayNo, string destination, double cost, DateTime departureDate, int noOfDays, int available)
        {
            this.holidayNo = holidayNo;
            this.destination = destination;
            this.cost = cost;
            this.departureDate = departureDate;
            this.noOfDays = noOfDays;
            this.available = available;
        }

        public int HolidayNo { get => holidayNo; set => holidayNo = value; }
        public string Destination { get => destination; set => destination = value; }
        public double Cost { get => cost; set => cost = value; }
        public DateTime DepartureDate { get => departureDate; set => departureDate = value; }
        public int NoOfDays { get => noOfDays; set => noOfDays = value; }
        public int Available { get => available; set => available = value; }


        public override string ToString()
        {
            string desc = $"{this.holidayNo}".PadRight(12);
            desc += this.destination.PadRight(25);
            desc += $"{this.departureDate:dd/MM/yyyy}".PadRight(17);
            desc += $"€{this.cost:n2}".PadRight(12);
            if (this.available == 1)
            {
                desc += "  Yes";
            }
            else
            {
                desc += "  No";
            }

            return desc;
        }

    }
}
