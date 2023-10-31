using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface IUserRepository
    {
        public Task<string> SignUpAsync(SignUpModel signUpModel);
        public Task<string> SignInAsync(SignInModel signInModel);
        public Task<Users> GetByIdAsync(string email);
    }
}
