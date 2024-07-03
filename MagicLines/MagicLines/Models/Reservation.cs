using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedFlightReservationSystem.Models
{
    public class Reservation
    {
        public User User { get; set; }
        public Flight Flight { get; set; }
        public string SeatClass { get; set; }
        public int PassengerCount { get; set; }
        public DateTime ReservationTime { get; set; }

        public override string ToString()
        {
            return $"{Flight.Route}, {Flight.GetFlightDate()}, Klasa: {SeatClass}, Pasażerów: {PassengerCount}, Rezerwacja: {ReservationTime:dd.MM.yyyy HH:mm}";
        }
    }
}
