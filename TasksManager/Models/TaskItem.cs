using System.ComponentModel.DataAnnotations;

namespace TasksManager.Models
{
    public class TaskItem
    {

        public class TaskStatus
        {
            public TaskStatus(string value) { Value = value; }

            public string Value { get; private set; }

            public static TaskStatus Done { get { return new TaskStatus("Done"); } }
            public static TaskStatus Todo { get { return new TaskStatus("Todo"); } }
            public static TaskStatus InProgress { get { return new TaskStatus("InProgress"); } }
            public static TaskStatus WontDo { get { return new TaskStatus("WontDo"); } }

            public static List<TaskStatus> All { 
                get { 
                    List<TaskStatus> All = new();
                    All.Add(Done);
                    All.Add(Todo);
                    All.Add(InProgress);
                    All.Add(WontDo);
                    return All; 
                } 
            }

            public override string ToString()
            {
                return Value;
            }
        }

        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public string AuthorUsername { get; set; }
        public string AssigneeUsername { get; set; }
        public bool CanBeDeleted { get; set; }
    }
}