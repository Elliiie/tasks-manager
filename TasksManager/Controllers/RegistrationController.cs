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
                    command.Parameters.AddWithValue("@username", user.Username.Trim());
                    command.Parameters.AddWithValue("@email", user.Email.Trim());
                    command.Parameters.AddWithValue("@password", StringHelper.ComputeHash(user.Password));
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
                command.Parameters.AddWithValue("@email", email.Trim());

                int rows = command.ExecuteNonQuery();
                if (rows > 0)
                {
                    IsUserExist = true;
                }
            }
        }
        return IsUserExist;
    }
}