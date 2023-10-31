using System.ComponentModel.DataAnnotations;

namespace TimeTable.DataContext.Models
{
    public class ClassModel
    {
        [Required]
        public string NameClass { get; set; } = null;
        [Required]
        public DateTime Year_Of_Admission { get; set; }
        [Required] 
        
        public string Course { get; set; } = null;
        public string DescriptionClass { get; set; } = null;
    }

    public class ReportClass : ClassModel
    {
        public Guid Id { get; set; }
        public int UsedState { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set;}
    }
}
