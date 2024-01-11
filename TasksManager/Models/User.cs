using System.ComponentModel;

namespace TasksManager.Models
{

    public class Role
    {
        private Role(string value) { Value = value; }

        public string Value { get; private set; }

        public static Role Admin { get { return new Role("Admin"); } }
        public static Role User { get { return new Role("User"); } }
        
        public override string ToString()
        {
            return Value;
        }
    }

    public class User 
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public List<TaskItem> Tasks { get; set; }
    }
}