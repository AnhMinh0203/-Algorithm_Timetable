using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface IChangerPasswordrepons
    {
        public Task<string> ChangerPasswordreponsAsync(ChangerPasswordModel changerPassword, string token);

    }
}
