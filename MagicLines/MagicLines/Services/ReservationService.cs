using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedFlightReservationSystem.Models;

public class ReservationService
{
    private List<AdvancedFlightReservationSystem.Models.Reservation> reservations = new List<AdvancedFlightReservationSystem.Models.Reservation>();

    public void LoadReservations()
    {
        // z pliku, todo
    }

    public void SaveReservations()
    {
        // do pliku, todo
    }

    public void AddReservation(AdvancedFlightReservationSystem.Models.User user, AdvancedFlightReservationSystem.Models.Flight flight, string seatClass, int passengerCount)
    {
        var reservation = new AdvancedFlightReservationSystem.Models.Reservation
        {
            User = user,
            Flight = flight,
            SeatClass = seatClass,
            PassengerCount = passengerCount,
            ReservationTime = DateTime.Now
        };
        reservations.Add(reservation);
        SaveReservations();
    }

    public List<AdvancedFlightReservationSystem.Models.Reservation> GetUserReservations(AdvancedFlightReservationSystem.Models.User user)
    {
        return reservations.Where(r => r.User.Username == user.Username).ToList();
    }
}
