namespace TimeTable.DataContext.Data
{
    public class Subject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public int appear { get; set; }
        public string course_code { get; set; }
        public string Description { get; set; }
    }
}
