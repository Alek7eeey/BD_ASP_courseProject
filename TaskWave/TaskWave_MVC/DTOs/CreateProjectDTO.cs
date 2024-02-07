namespace TaskWave_MVC.DTOs
{
    public class CreateProjectDTO
    {
        public int? id { get; set; }
        public string name { get; set; }
        public DateOnly toDate { get; set; }
        public string description { get; set; }
        public IFormFile photo { get; set; }
        public IFormFile document { get; set; }

        public List<String> tasks { get; set; }

    }
}
