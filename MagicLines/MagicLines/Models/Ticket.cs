using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedFlightReservationSystem.Models
{
    public class Ticket
    {
        public int Number { get; set; }
        public string Route { get; set; }
        public string Seat { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"Bilet nr {Number}, Trasa: {Route}, Klasa: {Seat}, Cena: {Price} PLN";
        }
    }
}
