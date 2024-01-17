namespace TasksManager.Models.ViewModels
{
    public class TasksViewModel
    {
        public List<TaskItem> TasksList { get; set; }
        public TaskItem Task { get; set; }
        public List<string> Usernames { get; set; }
        public string SelectedAssignee { get; set; }
        public string SelectedStatus { get; set; }

    }
}