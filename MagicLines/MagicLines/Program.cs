using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    private static Dictionary<int, Flight> flights = new Dictionary<int, Flight>();
    private static Dictionary<int, Ticket> tickets = new Dictionary<int, Ticket>();
    private static int ticketNumber = 1000;

    static void Main(string[] args)
    {
        InitializeFlights();
        LoadFlightData();
        LoadTicketData();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("1. Zarezerwuj lot");
            Console.WriteLine("2. Sprawdź lot");
            Console.WriteLine("\n0. Wyjdź");
            Console.Write("\nWybierz opcję: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    BookFlight();
                    break;
                case "2":
                    CheckFlight();
                    break;
                case "0":
                    SaveFlightData();
                    SaveTicketData();
                    return;
                default:
                    Console.WriteLine("Niepoprawna opcja, spróbuj ponownie. \nNaciśnij dowolny klawisz, żeby wrócić do menu");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static void InitializeFlights()
    {
        string filePath = "flights.txt";
        if (!File.Exists(filePath)) return;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            int flightCounter = 1;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                string route = parts[0];
                int basePrice = int.Parse(parts[1]);

                flights[flightCounter] = new Flight(route, basePrice);
                flightCounter++;
            }
        }
    }

    private static void BookFlight()
    {
        Console.Clear();
        foreach (var flight in flights)
        {
            Console.WriteLine($"{flight.Key}. {flight.Value.Route} - Odlot: {flight.Value.GetFlightDate()}");
        }
        Console.WriteLine("\n0. Wstecz");


        Console.WriteLine("\nWybierz trasę:");
        int routeChoice;
        if (!int.TryParse(Console.ReadLine(), out routeChoice) || routeChoice < 1 || routeChoice > 9)
        {
            Console.WriteLine("Niepoprawna opcja. \nNaciśnij dowolny klawisz, żeby wrócić do menu");
            Console.ReadKey();
            return;
        }
        if (routeChoice == 0) return;

        if (!flights.ContainsKey(routeChoice))
        {
            Console.WriteLine("Nie ma takiej trasy.");
            Console.ReadKey();
            return;
        }
        Console.Clear();
        Flight selectedFlight = flights[routeChoice];
        selectedFlight.DisplaySeats();
        Console.WriteLine("\n0. Wstecz");
        Console.WriteLine("\nWybierz klasę biletu: ");
        string classOption = Console.ReadLine();

        if (classOption == "0") return;

        Console.Clear();

        // Pobieranie danych osobowych
        Console.Write("Podaj imię: ");
        string firstName = Console.ReadLine();
        Console.Clear();
        Console.Write("Podaj nazwisko: ");
        string lastName = Console.ReadLine();
        Console.Clear();
        Console.Write("Podaj adres e-mail: ");
        string email = Console.ReadLine();
        Console.Clear();

        Ticket ticket = selectedFlight.BookSeat(classOption);
        if (ticket != null)
        {
            ticket.Number = ticketNumber++;
            ticket.FirstName = firstName;
            ticket.LastName = lastName;
            ticket.Email = email;
            tickets.Add(ticket.Number, ticket);
            SaveFlightData();
            SaveTicketData();
            Console.Clear();
            Console.WriteLine($"Zarezerwowano bilet: {ticket}");
            Console.WriteLine("Naciśnij dowolny klawisz, żeby wrócić do menu");
            if (Console.ReadLine() == "") return;
        }
    }

    private static void CheckFlight(string error = "")
    {
        Console.Clear();
        if (error != "")
        {
            Console.WriteLine(error);
        }
        Console.WriteLine("Podaj numer biletu: ");
        int number;
        if (!int.TryParse(Console.ReadLine(), out number))
        {
            Console.WriteLine("\nPodano nieprawidłowy numer biletu.");
            CheckFlight();
            return;
        }

        if (tickets.ContainsKey(number))
        {
            Console.Clear();
            Ticket ticket = tickets[number];
            Console.WriteLine($"Dane biletu: {ticket}");
            Console.WriteLine("1. Anuluj rezerwację");
            Console.WriteLine("0. Wstecz");
            Console.Write("\nWybierz opcję: ");
            string option = Console.ReadLine();

            if (option == "1")
            {
                Flight flight = FindFlightByRoute(ticket.Route);
                if (flight != null)
                {
                    flight.CancelSeat(ticket.Seat);
                    tickets.Remove(number);
                    SaveFlightData();
                    SaveTicketData();
                    Console.Clear();
                    Console.WriteLine("Rezerwacja anulowana. \nKoszt anulacji: " + ticket.Price * 0.1 + " PLN");
                    Console.WriteLine("\n0. Wstecz");
                    if (Console.ReadLine() == "0") return;
                }
                else
                {
                    Console.WriteLine("Nie znaleziono lotu odpowiadającego temu biletowi. \nNaciśnij dowolny klawisz, żeby wrócić do menu");
                    Console.ReadKey();
                }
            }
            else if (option == "0")
            {
                return;
            }
        }
        else
        {
            CheckFlight("Nie znaleziono biletu o podanym numerze.");
        }
    }

    private static Flight FindFlightByRoute(string route)
    {
        foreach (var flight in flights)
        {
            if (flight.Value.Route == route)
                return flight.Value;
        }
        return null;
    }

    private static void SaveFlightData()
    {
        string filePath = "flight_data.txt";
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var flight in flights)
            {
                writer.WriteLine($"{flight.Key},{flight.Value.Route},{flight.Value.BasePrice},{flight.Value.SeatsToString()}");
            }
        }
    }

    private static void LoadFlightData()
    {
        string filePath = "flight_data.txt";
        if (!File.Exists(filePath)) return;

        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split(',');
                int flightKey = int.Parse(parts[0]);
                string route = parts[1];
                int basePrice = int.Parse(parts[2]);
                var seats = new Dictionary<string, int>
                {
                    { "Business", int.Parse(parts[3]) },
                    { "Economy Plus", int.Parse(parts[4]) },
                    { "Economy", int.Parse(parts[5]) },
                    { "Economy (Window)", int.Parse(parts[6]) }
                };

                flights[flightKey] = new Flight(route, basePrice, seats);
            }
        }
    }

    private static void SaveTicketData()
    {
        string filePath = "tickets_data.json";
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(tickets, options);
        File.WriteAllText(filePath, json);
    }

    private static void LoadTicketData()
    {
        string filePath = "tickets_data.json";
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        tickets = JsonSerializer.Deserialize<Dictionary<int, Ticket>>(json);
        if (tickets != null && tickets.Count > 0)
        {
            ticketNumber = tickets.Last().Key + 1;
        }
    }
}

class Flight
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
        Console.WriteLine($"1. Economy: {BasePrice} PLN (Wolny miejsc: {seats["Economy"]})");
        Console.WriteLine($"2. Economy (miejsce przy oknie): {BasePrice * 1.2} PLN (Wolnych miejsc: {seats["Economy (Window)"]})");
        Console.WriteLine($"3. Economy Plus: {BasePrice * 2.0} PLN (Wolnych miejsc: {seats["Economy Plus"]})");
        Console.WriteLine($"4. Business: {BasePrice * 3.0} PLN (Wolnych miejsc: {seats["Business"]})");
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
            Console.WriteLine("Niepoprawna opcja lub brak miejsc w wybranej klasie. \nNaciśnij dowolny klawisz, żeby wrócić do menu");
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

class Ticket
{
    public int Number { get; set; }
    public string Route { get; set; }
    public string Seat { get; set; }
    public double Price { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public override string ToString()
    {
        return $"Bilet nr {Number} \nTrasa: {Route} \nKlasa: {Seat} \nCena: {Price} PLN \nImię: {FirstName} \nNazwisko: {LastName} \nEmail: {Email} \n";
    }
}
