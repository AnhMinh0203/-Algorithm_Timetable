using System.ComponentModel.DataAnnotations;

namespace TimeTable.DataContext.Models
{
    public class ChangerPasswordModel
    {
        [Required]
        public string PassWordHas { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmNewPassword { get; set;}
    }
}
