namespace TimeTable.DataContext.Data
{
    public class LectureSchedure
    {
        public Guid Id { get; set; }
        public Guid IdClass { get; set; }
        public Guid IdClassRoom { get; set; }
        public Guid IdSubject { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set;}
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string SchoolShift { get; set; }
    }

    public class LectureSchedure__test
    {
        public Guid IdLecture_Schedule { get; set; }
        public string LopHoc { get; set; }
        public string PhongHoc { get; set; }
        public string MonHoc { get; set; }
        public List<LichHoc> LichHocTongJson { get; set; }
    }

    public class LichHoc
    {
        public string dayStudy { get; set; }
        public string shiftStudy { get; set; }

    }
}
