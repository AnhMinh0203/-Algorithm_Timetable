namespace TimeTable.DataContext.Data
{
    public class Class
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Year_Of_Admission { get; set; }
        public string Course { get; set; }
        public int UsedState { get; set; }
        public string Description { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
