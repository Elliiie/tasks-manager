using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Please enter username")] 
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter a valid email")] 
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter password")] 
        public string Password { get; set; }
        public Role Role { get; set; }
        public List<TaskItem> Tasks { get; set; }
    }
}