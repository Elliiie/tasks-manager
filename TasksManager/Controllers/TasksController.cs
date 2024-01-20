using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using TasksManager.Models;
using TasksManager.Models.ViewModels;

namespace TasksManager.Controllers;

public class TasksController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Username") != null) {
            var taskViewModel = new TasksViewModel {
                TasksList = FetchAllTasks(),
                Usernames = FetchUserIfAllowed()
            };
            return View(taskViewModel);
        }
        return Redirect("http://localhost:5041/");
    }

    internal List<TaskItem> FetchAllTasks()
    {
        List<TasksManager.Models.TaskItem> tasks = new();

        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "SELECT * FROM Task WHERE Author=@author OR Assignee=@author";
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
                                    AuthorUsername = reader.GetString(2),
                                    AssigneeUsername = reader.IsDBNull(3) ? "NO ASSIGNEE" : reader.GetString(3),
                                    Title = reader.GetString(4),
                                    Status = new TaskItem.TaskStatus(reader.GetString(5)),
                                    CanBeDeleted = (reader.GetString(2) == reader.GetString(3)) || havePermission()
                                }
                            );
                        }
                    }
                }
            }
        }
        return tasks;
    }

    internal List<String> FetchUserIfAllowed() 
    {
        List<String> Usernames = new();
        if (!havePermission()) { return Usernames; }

        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "SELECT * FROM User WHERE NOT Username=@username AND Role=@role";
                command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("Username"));
                command.Parameters.AddWithValue("@role", Role.User.Value);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Usernames.Add(reader.GetString(0));
                        }
                    }
                }
            }
        }
        return Usernames;
    }

    internal bool havePermission()
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = "SELECT * FROM User WHERE Username=@username";
                command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("Username"));

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                            return reader.GetString(3) == Role.Admin.Value;
                    }
                    
                }
            }
        }
        return false;
    }

    public RedirectResult Insert(TasksManager.Models.TaskItem task)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"INSERT INTO Task (Description, Author, Assignee, Title, Status) VALUES (@description, @username, @username, @title, @status)";
                command.Parameters.AddWithValue("@title", task.Title);
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@username", HttpContext.Session.GetString("Username"));
                command.Parameters.AddWithValue("@status", TaskItem.TaskStatus.Todo.Value);
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

    [HttpPost]
    public RedirectResult Assign(string selectedAssignee, int taskId)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"UPDATE Task SET Assignee=@assignee WHERE Id={taskId}";
                command.Parameters.AddWithValue("@assignee", selectedAssignee);
                command.ExecuteNonQuery();
            }
        }

        return Redirect("http://localhost:5041/Tasks");  
    }

    [HttpPost]
    public RedirectResult ChangeStatus(string selectedStatus, int taskId)
    {
        using (SqliteConnection connection = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var command = connection.CreateCommand())
            {
                connection.Open();
                command.CommandText = $"UPDATE Task SET Status=@status WHERE Id={taskId}";
                command.Parameters.AddWithValue("@status", selectedStatus);
                command.ExecuteNonQuery();
            }
        }

        return Redirect("http://localhost:5041/Tasks");  
    }
}