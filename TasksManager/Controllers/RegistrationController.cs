using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TasksManager.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;
using TasksManager.Models;

namespace TasksManager.Controllers;

public class RegistrationController : Controller
{
     public IActionResult Index()
    {
        return View();
    }

    public RedirectResult Insert(TasksManager.Models.User user)
    {
        if (!IsUserExist(user.Email))
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
            {
                using (var command = connection.CreateCommand())
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO User VALUES (@username, @email, @password, @role)";
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.Parameters.AddWithValue("@email", user.Email);

                    using (SHA512 shaM = new SHA512Managed())
                    {
                        string passwordHash = GetStringFromHash(shaM.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
                        command.Parameters.AddWithValue("@password", passwordHash);
                    }

                    command.Parameters.AddWithValue("@role", Role.User.Value);

                    command.ExecuteNonQuery();
                }
            }

            return Redirect("http://localhost:5041/");
        }
        else
        {
            ModelState.AddModelError("", "User with same username already exist!");

            return Redirect("http://localhost:5041/Registration");
        }

        
    }

    private bool IsUserExist(string email)
    {
        bool IsUserExist = false;
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "SELECT * FROM USER WHERE Email=@email";
                command.Parameters.AddWithValue("@email", email);

                int rows = command.ExecuteNonQuery();
                if (rows > 0)
                {
                    IsUserExist = true;
                }
            }
        }
        return IsUserExist;
    }

    private static string GetStringFromHash(byte[] hash)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            result.Append(hash[i].ToString("X2"));
        }
        return result.ToString();
    }
}