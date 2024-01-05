namespace TasksManager.Models
{
    public class User 
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<TaskItem> Tasks { get; set; }
    }
}