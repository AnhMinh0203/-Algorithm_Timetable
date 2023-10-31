using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface ISubjectRepons
    {
        public Task<string> AddClassAsync(SubjectModel subjectModel, string token);
        public Task<string> DeleteClassAsync(Guid roomId);
        public Task<(List<Subject>, int)> GetAllClassAsync(int pageIndex, int pageSize);
        public Task<(List<Subject>, int)> GetClassByIdAsync(string roomId, int pageIndex, int pageSize);
        public Task<string> UpdateClassAsync(Guid roomId, SubjectModel subjectModel, string token);
        public Task<byte[]> ExportToExcelAsync();
    }
}
