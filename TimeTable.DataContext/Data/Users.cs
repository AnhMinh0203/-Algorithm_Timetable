namespace TimeTable.DataContext.Data
{
    public class Users
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PassWordHas { get; set; }
        public string Phone{ get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Avata { get; set; }
        public int UsedState { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Token { get; set; }
    }
}
