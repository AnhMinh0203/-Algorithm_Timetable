using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface ILectureSchedureRepons
    {
        //public Task<List<LectureSchedureModel>> GetAllSchedureReponsAsync();
        public Task<(List<Lecture_ScheduleUserModel>, int)> GetRegisteredCalendarAsync (string token, int pageIndex, int pageSize);
        public Task<(List<Lecture_ScheduleUserModel>, int)> GetSchedureByIdReponsAsync(string token, string search, int pageIndex, int pageSize);
        public Task<(List<Lecture_ScheduleUserModel>, int)> GetAllSchedureReponsAsync(int pageIndex, int pageSize, int check, string Name);
        public Task<string> UserRegisterEdCalendarAsync(string token, Guid idSchedure, string Course_Code);
    }
}
