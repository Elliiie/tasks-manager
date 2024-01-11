namespace TasksManager.Models
{

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public string Status {
            get {
                return IsDone ? "DONE" : "NOT DONE";
            }
        }
        public string AuthorUsername { get; set; }
        public string AssigneeUsername { get; set; }
    }
}