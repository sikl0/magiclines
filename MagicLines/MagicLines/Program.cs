using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using BCrypt.Net;
using System.Xml;

// Klasy pomocnicze
namespace AdvancedFlightReservationSystem.Models
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public int LoyaltyPoints { get; set; }
    }

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
            Console.WriteLine($"1. Economy: {BasePrice} PLN ({seats["Economy"]})");
            Console.WriteLine($"2. Economy (miejsce przy oknie): {BasePrice * 1.2} PLN ({seats["Economy (Window)"]})");
            Console.WriteLine($"3. Economy Plus: {BasePrice * 2.0} PLN ({seats["Economy Plus"]})");
            Console.WriteLine($"4. Business: {BasePrice * 3.0} PLN ({seats["Business"]})");
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

// Serwisy
public class UserService
{
    private List<AdvancedFlightReservationSystem.Models.User> users = new List<AdvancedFlightReservationSystem.Models.User>();
    private readonly string usersFilePath = "users.json";

    public void LoadUsers()
    {
        if (File.Exists(usersFilePath))
        {
            string json = File.ReadAllText(usersFilePath);
            users = JsonConvert.DeserializeObject<List<AdvancedFlightReservationSystem.Models.User>>(json) ?? new List<AdvancedFlightReservationSystem.Models.User>();
        }
    }

    public void SaveUsers()
    {
        string json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(usersFilePath, json);
    }

    public bool RegisterUser(string username, string password, string email)
    {
        if (users.Any(u => u.Username == username || u.Email == email))
        {
            return false;
        }

        var user = new AdvancedFlightReservationSystem.Models.User
        {
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        users.Add(user);
        SaveUsers();
        return true;
    }

    public AdvancedFlightReservationSystem.Models.User AuthenticateUser(string username, string password)
    {
        var user = users.FirstOrDefault(u => u.Username == username);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return user;
        }
        return null;
    }

    public bool ChangePassword(AdvancedFlightReservationSystem.Models.User user, string newPassword)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        SaveUsers();
        return true;
    }

    public bool ChangeEmail(AdvancedFlightReservationSystem.Models.User user, string newEmail)
    {
        if (users.Any(u => u.Email == newEmail))
        {
            return false;
        }

        user.Email = newEmail;
        SaveUsers();
        return true;
    }

    public void AddLoyaltyPoints(AdvancedFlightReservationSystem.Models.User user, int points)
    {
        user.LoyaltyPoints += points;
        SaveUsers();
    }
}

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

public class ReservationService
{
    private List<AdvancedFlightReservationSystem.Models.Reservation> reservations = new List<AdvancedFlightReservationSystem.Models.Reservation>();

    public void LoadReservations()
    {
        // Simulacja ładowania rezerwacji z bazy danych lub pliku
    }

    public void SaveReservations()
    {
        // Simulacja zapisywania rezerwacji do bazy danych lub pliku
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

public class PaymentService
{
    public bool ProcessPayment(AdvancedFlightReservationSystem.Models.User user, double amount)
    {
        // Simulacja przetwarzania płatności
        // Można dodać tutaj integrację z bramką płatności
        return true;
    }
}

// Główna klasa programu
class Program
{
    private static UserService userService = new UserService();
    private static FlightService flightService = new FlightService();
    private static ReservationService reservationService = new ReservationService();
    private static PaymentService paymentService = new PaymentService();
    private static AdvancedFlightReservationSystem.Models.User loggedInUser;

    static void Main(string[] args)
    {
        userService.LoadUsers();
        flightService.LoadFlights();
        reservationService.LoadReservations();

        while (true)
        {
            Console.Clear();
            if (loggedInUser == null)
            {
                Console.WriteLine("1. Zarejestruj się");
                Console.WriteLine("2. Zaloguj się");
                Console.WriteLine("0. Wyjdź");
                Console.Write("Wybierz opcję: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        Register();
                        break;
                    case "2":
                        Login();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Niepoprawna opcja, spróbuj ponownie.");
                        Console.ReadKey();
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Zalogowany jako: {loggedInUser.Username}");
                Console.WriteLine("1. Zarezerwuj lot");
                Console.WriteLine("2. Pokaż moje rezerwacje");
                Console.WriteLine("3. Zmień hasło");
                Console.WriteLine("4. Zmień email");
                Console.WriteLine("5. Wyloguj się");
                Console.WriteLine("0. Wyjdź");
                Console.Write("Wybierz opcję: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        BookFlight();
                        break;
                    case "2":
                        ShowReservations();
                        break;
                    case "3":
                        ChangePassword();
                        break;
                    case "4":
                        ChangeEmail();
                        break;
                    case "5":
                        loggedInUser = null;
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Niepoprawna opcja, spróbuj ponownie.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    private static void Register()
    {
        Console.Clear();
        Console.Write("Podaj nazwę użytkownika: ");
        string username = Console.ReadLine();
        Console.Write("Podaj hasło: ");
        string password = Console.ReadLine();
        Console.Write("Podaj email: ");
        string email = Console.ReadLine();

        if (userService.RegisterUser(username, password, email))
        {
            Console.WriteLine("Rejestracja zakończona sukcesem.");
        }
        else
        {
            Console.WriteLine("Rejestracja nie powiodła się. Nazwa użytkownika lub email jest już w użyciu.");
        }

        Console.ReadKey();
    }

    private static void Login()
    {
        Console.Clear();
        Console.Write("Podaj nazwę użytkownika: ");
        string username = Console.ReadLine();
        Console.Write("Podaj hasło: ");
        string password = Console.ReadLine();

        loggedInUser = userService.AuthenticateUser(username, password);
        if (loggedInUser == null)
        {
            Console.WriteLine("Niepoprawna nazwa użytkownika lub hasło.");
            Console.ReadKey();
        }
    }

    private static void BookFlight()
    {
        Console.Clear();
        Console.WriteLine("Wybierz trasę:");
        var flights = flightService.GetFlights();
        foreach (var flight in flights)
        {
            Console.WriteLine($"{flight.Key}. {flight.Value.Route} - Odlot: {flight.Value.GetFlightDate()}");
        }
        Console.WriteLine("9. Wstecz");

        Console.Write("Wybierz opcję: ");
        int routeChoice;
        if (!int.TryParse(Console.ReadLine(), out routeChoice) || routeChoice < 1 || routeChoice > 9)
        {
            Console.WriteLine("Niepoprawna opcja.");
            Console.ReadKey();
            return;
        }
        if (routeChoice == 9) return;

        if (!flights.ContainsKey(routeChoice))
        {
            Console.WriteLine("Nie ma takiej trasy.");
            Console.ReadKey();
            return;
        }

        var selectedFlight = flights[routeChoice];
        Console.Clear();
        selectedFlight.DisplaySeats();
        Console.WriteLine("9. Wstecz");
        Console.Write("Wybierz klasę biletu: ");
        string classOption = Console.ReadLine();

        if (classOption == "9") return;

        Console.Clear();
        var ticket = selectedFlight.BookSeat(classOption);
        if (ticket != null)
        {
            double totalCost = ticket.Price;

            Console.WriteLine($"Całkowity koszt: {totalCost} PLN");

            if (paymentService.ProcessPayment(loggedInUser, totalCost))
            {
                reservationService.AddReservation(loggedInUser, selectedFlight, ticket.Seat, 1);
                Console.WriteLine("Rezerwacja zakończona sukcesem.");
            }
            else
            {
                Console.WriteLine("Płatność nie powiodła się.");
            }
        }

        Console.WriteLine("9. Wstecz");
        if (Console.ReadLine() == "9") return;
    }

    private static void ShowReservations()
    {
        Console.Clear();
        var reservations = reservationService.GetUserReservations(loggedInUser);

        if (reservations.Any())
        {
            Console.WriteLine("Twoje rezerwacje:");
            foreach (var reservation in reservations)
            {
                Console.WriteLine(reservation);
            }
        }
        else
        {
            Console.WriteLine("Brak rezerwacji.");
        }

        Console.WriteLine("9. Wstecz");
        if (Console.ReadLine() == "9") return;
    }

    private static void ChangePassword()
    {
        Console.Clear();
        Console.Write("Nowe hasło: ");
        string newPassword = Console.ReadLine();

        if (userService.ChangePassword(loggedInUser, newPassword))
        {
            Console.WriteLine("Hasło zmienione pomyślnie.");
        }

        Console.ReadKey();
    }

    private static void ChangeEmail()
    {
        Console.Clear();
        Console.Write("Nowy email: ");
        string newEmail = Console.ReadLine();

        if (userService.ChangeEmail(loggedInUser, newEmail))
        {
            Console.WriteLine("Email zmieniony pomyślnie.");
        }
        else
        {
            Console.WriteLine("Zmiana emaila nie powiodła się. Podany email jest już w użyciu.");
        }

        Console.ReadKey();
    }
}
