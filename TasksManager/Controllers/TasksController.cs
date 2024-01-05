using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TasksManager.Models.ViewModels;

namespace TasksManager.Controllers;

public class TasksController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Username") != null) {
            var taskViewModel = FetchAllTasks();
            return View(taskViewModel);
        }
        return Redirect("http://localhost:5041/");
    }

    internal TasksViewModel FetchAllTasks()
    {
        List<TasksManager.Models.TaskItem> tasks = new();

        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "SELECT * FROM Task WHERE Author=@author";
                command.Parameters.AddWithValue("@author", HttpContext.Session.GetString("Username"));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tasks.Add(
                                new Models.TaskItem
                                {
                                    Id = reader.GetInt32(0),
                                    Description = reader.GetString(1),
                                    IsDone = reader.GetBoolean(2),
                                }
                            );
                        }
                    }
                }
            }
        }

        return new TasksViewModel
                    {
                        TasksList = tasks
                    };
    }

    public RedirectResult Insert(TasksManager.Models.TaskItem task)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"INSERT INTO Task (Description, Done, Author) VALUES (@description, false, @username)";
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("Username"));
                command.ExecuteNonQuery();
            }
        }

        return Redirect("http://localhost:5041/Tasks");
    }

    public ActionResult Delete(int Id)
    {
         using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"DELETE FROM Task WHERE Id={Id}";
                command.ExecuteNonQuery();
            }
        }
        return Redirect("http://localhost:5041/Tasks"); 
    }

    public ActionResult IsDone(int Id, bool Done)
    {
         using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"UPDATE Task SET Done={Done} WHERE Id={Id}";
                command.ExecuteNonQuery();
            }
        }
        return Redirect("http://localhost:5041/Tasks"); 
    }

    public ActionResult Edit(int Id)
    {
        Models.TaskItem task = new();

        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"SELECT * FROM Task WHERE Id={Id}";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            task.Id = reader.GetInt32(0);
                            task.Description = reader.GetString(1);
                        }
                    }
                }
            }
        }
        return View(task);
    }

    [HttpPost]
    public RedirectResult Edit(TasksManager.Models.TaskItem task)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"UPDATE Task SET Description=@description WHERE Id={task.Id}";
                command.Parameters.AddWithValue("@description", task.Description);
                command.ExecuteNonQuery();
            }
        }

        return Redirect("http://localhost:5041/Tasks");  
    }
}