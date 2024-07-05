using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedFlightReservationSystem.Models
{
    public class Flight
    {
        public string Route { get; private set; }
        public int BasePrice { get; private set; }
        private Dictionary<string, int> seats = new Dictionary<string, int>();
        private DateTime flightDate;
        private Random random = new Random();

        public Flight(string route, int basePrice)
        {
            Route = route;
            BasePrice = basePrice;
            seats["Business"] = 18;
            seats["Economy Plus"] = 18;
            seats["Economy"] = 96;
            seats["Economy (Window)"] = 52;
            flightDate = GenerateFlightDate();
        }

        public Flight(string route, int basePrice, Dictionary<string, int> seats)
        {
            Route = route;
            BasePrice = basePrice;
            this.seats = seats;
            flightDate = GenerateFlightDate();
        }

        private DateTime GenerateFlightDate()
        {
            int daysToAdd = random.Next(1, 11);
            int hoursToAdd = random.Next(0, 24);
            int minutesToAdd = random.Next(0, 12) * 5;
            return DateTime.Today.AddDays(daysToAdd).AddHours(hoursToAdd).AddMinutes(minutesToAdd);
        }

        public string GetFlightDate()
        {
            return flightDate.ToString("yyyy-MM-dd HH:mm");
        }

        public void DisplaySeats()
        {
            Console.WriteLine($"1. Economy: {BasePrice} PLN ({seats["Economy"]})\n");
            Console.WriteLine($"2. Economy (miejsce przy oknie): {BasePrice * 1.2} PLN ({seats["Economy (Window)"]})\n");
            Console.WriteLine($"3. Economy Plus: {BasePrice * 2.0} PLN ({seats["Economy Plus"]})\n");
            Console.WriteLine($"4. Business: {BasePrice * 3.0} PLN ({seats["Business"]})\n");
        }

        public Ticket BookSeat(string classOption)
        {
            string selectedClass = classOption switch
            {
                "1" => "Economy",
                "2" => "Economy (Window)",
                "3" => "Economy Plus",
                "4" => "Business",
                _ => null
            };

            if (selectedClass == null || seats[selectedClass] == 0)
            {
                Console.WriteLine("Niepoprawna opcja lub brak miejsc w wybranej klasie.");
                return null;
            }

            seats[selectedClass]--;
            double priceMultiplier = selectedClass switch
            {
                "Economy" => 1.0,
                "Economy (Window)" => 1.2,
                "Economy Plus" => 2.0,
                "Business" => 3.0,
                _ => 1.0
            };
            double price = BasePrice * priceMultiplier;
            return new Ticket
            {
                Route = Route,
                Seat = selectedClass,
                Price = price
            };
        }

        public void CancelSeat(string seatClass)
        {
            if (seats.ContainsKey(seatClass))
            {
                seats[seatClass]++;
            }
        }

        public string SeatsToString()
        {
            return $"{seats["Business"]},{seats["Economy Plus"]},{seats["Economy"]},{seats["Economy (Window)"]}";
        }
    }
}
