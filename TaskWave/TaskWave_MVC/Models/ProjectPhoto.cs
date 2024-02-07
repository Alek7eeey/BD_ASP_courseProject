namespace TaskWave_MVC.Models
{
    public class ProjectPhoto
    {
        public int? id { get; set; }
        public int? projectId { get; set; }
        public string? fileName { get; set; }
        public byte[]? photo { get; set; }
    }
}
