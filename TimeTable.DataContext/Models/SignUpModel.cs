using System.ComponentModel.DataAnnotations;

namespace TimeTable.DataContext.Models
{
    public class SignUpModel
    {
        //public Guid Id { get; set; }
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;     
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string ConfirmPassword { get; set; } = null!;
        [Required]
        public string TypeAccount { get; set; }
        public string Gender { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Avata { get; set; } = null!;
    }
}
