namespace TimeTable.DataContext.Data
{
    public class ClassRooms
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UsedState { get; set; }
        public string Description { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set;}
    }
}
