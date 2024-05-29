namespace TimeTable.DataContext.Models
{
    public class UserManagerModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int Phone { get; set; }
        public string? Type { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int UsedState { get; set; } 
        public string Description { get; set; }
        public string Avata { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set;}
    }

}
