using System.ComponentModel.DataAnnotations;

namespace TimeTable.DataContext.Models
{
    public class SignInModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string PassWordHas { get; set; } = null!;
    }
    public class Custommessage
    {
        public string status { get; set; }
        public string token { get; set; }
        public Guid Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string avata { get; set; }
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set;}
    }
}
