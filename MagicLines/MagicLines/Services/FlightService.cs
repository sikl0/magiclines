using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedFlightReservationSystem.Models;

public class FlightService
{
    private Dictionary<int, AdvancedFlightReservationSystem.Models.Flight> flights = new Dictionary<int, AdvancedFlightReservationSystem.Models.Flight>();
    private readonly string flightsFilePath = "flight_data.txt";

    public void LoadFlights()
    {
        // Nowe ładowanie lotów z pliku flight_data.txt
        if (File.Exists(flightsFilePath))
        {
            string[] lines = File.ReadAllLines(flightsFilePath);
            foreach (var line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 3)
                {
                    string route = parts[0];
                    int basePrice = int.Parse(parts[1]);
                    string[] seatParts = parts[2].Split(',');
                    var seats = new Dictionary<string, int>
                    {
                        { "Business", int.Parse(seatParts[0]) },
                        { "Economy Plus", int.Parse(seatParts[1]) },
                        { "Economy", int.Parse(seatParts[2]) },
                        { "Economy (Window)", int.Parse(seatParts[3]) }
                    };

                    flights.Add(flights.Count + 1, new AdvancedFlightReservationSystem.Models.Flight(route, basePrice, seats));
                }
            }
        }
    }

    public void SaveFlights()
    {
        using (StreamWriter sw = new StreamWriter(flightsFilePath))
        {
            foreach (var flight in flights.Values)
            {
                sw.WriteLine($"{flight.Route}|{flight.BasePrice}|{flight.SeatsToString()}");
            }
        }
    }


    public List<AdvancedFlightReservationSystem.Models.Flight> SearchFlights(string route)
    {
        return flights.Values.Where(f => f.Route.Contains(route)).ToList();
    }

    public Dictionary<int, AdvancedFlightReservationSystem.Models.Flight> GetFlights()
    {
        return flights;
    }
}

