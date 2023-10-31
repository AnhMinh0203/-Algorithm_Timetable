using TimeTable.DataContext.Data;

namespace TimeTable.DataContext.Models
{
    public class LectureSchedureModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SchoolShift { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
