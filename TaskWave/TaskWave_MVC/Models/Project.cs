

using TaskWave_MVC.Models.ENUMs;

namespace TaskWave_MVC.Models
{
    public class Project
    {
        public int? id { get; set; }
        public int? artistId { get; set; } //id исполнителя
        public int? creatorId { get; set; }
        public string name { get; set; }
        public DateOnly toDate { get; set; }
        public DateOnly fromDate { get; set; }
        public string description { get; set; }
        public ProjectENUM type { get; set; }
        public ProjectPhoto? photo { get; set; }
        public ProjectDocument? document { get; set; }
        public List<Task>? tasks { get; set; }

    }
}
