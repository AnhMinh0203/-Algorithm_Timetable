using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface IUserManagerRepons
    {
        public Task<(List<UserManagerModel>, int)> GetAllUserAsync();
        public Task<(List<UserManagerModel>, int)> GetUserByIdAsync(string id);
        public Task<string> DeleteUserByIdAsync(Guid id);
        public Task<string> UpdateUsedStatedByIdAsync(Guid id, int usedStated);
        public Task<string> LockAccount(string TypeAccount, int UsedStated);
    }
}
