using System.ComponentModel.DataAnnotations;

namespace TimeTable.DataContext.Models
{
    public class ClassRoomModel
    {
        [Required]
        public string Name { get; set; } = null;
        public string Description { get; set; } = null;
    }

    public class ReportClassRoom : ClassRoomModel
    {
        public Guid Id { get; set; }
        public int UsedState { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
