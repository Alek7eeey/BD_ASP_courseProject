namespace TaskWave_MVC.Models
{
    public class UserDescription
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string? email { get; set; }
        public string password { get; set; }
        public string login { get; set; }
        public string? company { get; set; }
        public string? telegram { get; set; }
        public string? city { get; set; }
        public byte[]? image { get; set; }
        public string? titlePhoto { get; set; }
    }
}
