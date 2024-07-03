using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedFlightReservationSystem.Models;

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
