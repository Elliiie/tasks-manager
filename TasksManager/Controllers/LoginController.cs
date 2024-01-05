using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TasksManager.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace TasksManager.Controllers;

public class LoginController : Controller
{
     public IActionResult Index()
    {
        return View();
    }

    public RedirectResult Insert(TasksManager.Models.User user)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"SELECT * FROM User WHERE (Username=@username AND Password=@password)";
                command.Parameters.AddWithValue("@username", user.Username);

                using (SHA512 shaM = new SHA512Managed())
                {
                    string passwordHash = GetStringFromHash(shaM.ComputeHash(Encoding.UTF8.GetBytes(user.Password)));
                    command.Parameters.AddWithValue("@password", passwordHash);
                }

                 using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        HttpContext.Session.SetString("Username", user.Username);
                        return Redirect("http://localhost:5041/Tasks");
                    }
                }
            }
        }

        return Redirect("http://localhost:5041");
    }

    public RedirectResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("http://localhost:5041");
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