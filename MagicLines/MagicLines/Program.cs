using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using BCrypt.Net;
using System.Xml;
using AdvancedFlightReservationSystem.Models;

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
                Console.WriteLine("WITAJ W 'MAGIC LINES'\n");
                Console.WriteLine("1. Zarejestruj się\n");
                Console.WriteLine("2. Zaloguj się\n");
                Console.WriteLine("0. Wyjdź\n");
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
                Console.WriteLine("MAGIC LINES\n");
                Console.WriteLine($"Zalogowany jako: {loggedInUser.Username}\n");
                Console.WriteLine("1. Zarezerwuj lot\n");
                Console.WriteLine("2. Pokaż moje rezerwacje\n");
                Console.WriteLine("3. Zmień hasło\n");
                Console.WriteLine("4. Zmień email\n");
                Console.WriteLine("5. Wyloguj się\n");
                Console.WriteLine("0. Wyjdź\n");
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
        Console.WriteLine("MAGICLINES\n");
        Console.Write("Podaj nazwę użytkownika: ");
        string username = Console.ReadLine();
        Console.Write("Podaj hasło: ");
        string password = Console.ReadLine();
        Console.Write("Podaj email: ");
        string email = Console.ReadLine();

        if (userService.RegisterUser(username, password, email))
        {
            Console.WriteLine("\nRejestracja zakończona sukcesem.");
        }
        else
        {
            Console.WriteLine("\nRejestracja nie powiodła się. Nazwa użytkownika lub email jest już w użyciu.");
        }

        Console.ReadKey();
    }

    private static void Login()
    {
        Console.Clear();
        Console.WriteLine("MAGIC LINES\n");
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
        Console.WriteLine("MAGIC LINES\n");
        Console.WriteLine("Wybierz trasę: \n");
        var flights = flightService.GetFlights();
        foreach (var flight in flights)
        {
            Console.WriteLine($"{flight.Key}. {flight.Value.Route} - Odlot: {flight.Value.GetFlightDate()}\n");
        }
        Console.WriteLine("9. Wstecz\n");

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
        Console.WriteLine("MAGICLINES\n");
        selectedFlight.DisplaySeats();
        Console.WriteLine("\n9. Wstecz\n");
        Console.Write("Wybierz klasę biletu: \n");
        string classOption = Console.ReadLine();

        if (classOption == "9") return;

        Console.Clear();
        var ticket = selectedFlight.BookSeat(classOption);
        if (ticket != null)
        {
            double totalCost = ticket.Price;
            Console.WriteLine("MAGIC LINES\n");
            Console.WriteLine($"Całkowity koszt: {totalCost} PLN\n");

            if (paymentService.ProcessPayment(loggedInUser, totalCost))
            {
                reservationService.AddReservation(loggedInUser, selectedFlight, ticket.Seat, 1);
                Console.WriteLine("Rezerwacja zakończona sukcesem.\n");
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
            Console.WriteLine("MAGIC LINES\n");
            Console.WriteLine("Twoje rezerwacje: \n");
            foreach (var reservation in reservations)
            {
                Console.WriteLine(reservation);
            }
        }
        else
        {
            Console.WriteLine("Brak rezerwacji.\n");
        }

        Console.WriteLine("9. Wstecz");
        if (Console.ReadLine() == "9") return;
    }

    private static void ChangePassword()
    {
        Console.Clear();
        Console.WriteLine("MAGIC LINES\n");
        Console.Write("Nowe hasło: ");
        string newPassword = Console.ReadLine();

        if (userService.ChangePassword(loggedInUser, newPassword))
        {
            Console.WriteLine("\nHasło zmienione pomyślnie.");
        }

        Console.ReadKey();
    }

    private static void ChangeEmail()
    {
        Console.Clear();
        Console.WriteLine("MAGIC LINES\n");
        Console.Write("Nowy email: ");
        string newEmail = Console.ReadLine();

        if (userService.ChangeEmail(loggedInUser, newEmail))
        {
            Console.WriteLine("\nEmail zmieniony pomyślnie.");
        }
        else
        {
            Console.WriteLine("Zmiana emaila nie powiodła się. Podany email jest już w użyciu.");
        }

        Console.ReadKey();
    }
}
