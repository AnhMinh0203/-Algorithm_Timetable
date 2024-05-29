namespace TimeTable.DataContext.Models
{
    public class Lecture_ScheduleManagerModel
    {
        public Guid IdLecture_Schedule { get; set; }
        public string Course_Code { get; set; }
        public string FullName { get; set; }
        public string LopHoc { get; set; }
        public string PhongHoc { get; set; }
        public string MonHoc { get; set; }
        public string LichHocTongList { get; set; }
    }
    public class SchedulingInputModel
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public DateTime DateCreate { get; set; }
        public int Count { get; set; }
        public List<Guid> Idclasses { get; set; }
        public List<Guid> IdclassRooms { get; set; }
        public List<Guid> Idsubjects { get; set; }
    }
    public class Lecture_ScheduleUserModel
    {
        public Guid IdLecture_Schedule { get; set; }
        public string Course_Code { get; set; }
        public string FullName { get; set; }
        public string LopHoc { get; set; }
        public string PhongHoc { get; set; }
        public string MonHoc { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public string LichHocTongList { get; set; }
    }
}
