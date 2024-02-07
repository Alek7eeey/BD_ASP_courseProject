namespace TaskWave_MVC.Models
{
    public class ReadyProject
    {
        public int? id { get; set; }
        public int? project_id { get; set; }
        public string project_name { get; set; }
        public DateOnly dateComplete { get; set; }
        public int? perfomer_user_id { get; set; }
        public string description { get; set; }
        public int? creatorId { get; set; }
        public DateOnly toDate { get; set; }
        public DateOnly fromDate { get; set; }
        public ProjectPhoto? photo { get; set; }
        public ProjectDocument? document { get; set; }
        public List<Task>? tasks { get; set; }
    }
}
