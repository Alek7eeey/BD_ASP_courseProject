namespace TaskWave_MVC.DTOs
{
    public class UserDescriptionDTO
    {
        public int idUser { get; set; }
        public string? email { get; set; }
        public string? company { get; set; }
        public string? telegram { get; set; }
        public string? city { get; set; }
        public IFormFile image { get; set; }
        public string? titlePhoto { get; set; }
    }
}
