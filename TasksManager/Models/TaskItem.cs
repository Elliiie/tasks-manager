namespace TasksManager.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public string AuthorUsername { get; set; }
        public string AssigneeUsername { get; set; }
    }
}