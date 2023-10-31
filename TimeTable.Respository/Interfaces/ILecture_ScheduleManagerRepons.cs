using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface ILecture_ScheduleManagerRepons
    {
        public Task<(List<Lecture_ScheduleManagerModel>, int)> GetAllLecture_ScheduleManagerAsync(int pageIndex, int pageSize);
        public Task<(List<Lecture_ScheduleManagerModel>, int)> GetLecture_ScheduleManagerByNameAsync(string name, int pageIndex, int pageSize);
        public Task<string> DeleteLecture_ScheduleManagerAsync(Guid id);
        public Task<string> UpdateLecture_ScheduleManagerAsync(Guid id, Lecture_ScheduleManagerModel lecture_ScheduleManagerModel);
        public Task<string> AddLecture_ScheduleManagerAsync(Lecture_ScheduleManagerModel model);
        public Task<List<LectureSchedureMapUserModel>> SchedulingAscync(SchedulingInputModel schedulingInputModel);
    }
}
