using TaskWave_MVC.Models.ENUMs;

namespace TaskWave_MVC.Models
{
    public class User
    {
        public int? id { get; set; }
        public string Login { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string? Description { get; set; }

        public RoleENUM Role { get; set; }
    }
}
