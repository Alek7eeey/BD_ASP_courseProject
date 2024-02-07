namespace TaskWave_MVC.DTOs
{
    public class UpdateProjectDTO
    {
        public int? id { get; set; }
        public string name { get; set; }
        public DateOnly toDate { get; set; }
        public string description { get; set; }
        public IFormFile photo { get; set; }
        public IFormFile document { get; set; }
        public List<TaskDTO> tasks { get; set; }
    }

    public class TaskDTO
    {
        public int? Id { get; set; }
        public string Description { get; set; }
    }
}
