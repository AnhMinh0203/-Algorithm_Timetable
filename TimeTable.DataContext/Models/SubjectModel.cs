namespace TimeTable.DataContext.Models
{
    public class SubjectModel
    {
        public string course_code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Description { get; set; }
    }

    public class ReportSubject : SubjectModel
    {
        public Guid Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set;}
    }
}
