namespace TimeTable.DataContext.Models
{
    public class EditAccountModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Avata { get; set; }
        public string Description { get; set; } 
    }

    public class EditAccountManagerModel : EditAccountModel
    {
        public string Email { get; set; }
        public int NumberPhone { get; set; }
        public string Avata { get; set; }
        public int UsedStated { get; set; }
    }
}
