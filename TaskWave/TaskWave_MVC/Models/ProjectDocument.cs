namespace TaskWave_MVC.Models
{
    public class ProjectDocument
    {
        public int? id { get; set; }
        public int? projectId { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public byte[] data { get; set; }
    }
}
