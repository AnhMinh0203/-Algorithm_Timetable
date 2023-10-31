using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface IEditAccountRepons
    {
        public Task<string> EditInforAccountAsync(EditAccountModel editAccountModel, string token);
        public Task<string> EditInforAccountManagerAsync(Guid id, EditAccountManagerModel editAccountManagerModel, string token);
    }
}
