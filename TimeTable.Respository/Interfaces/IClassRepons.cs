using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface IClassRepons
    {
        public Task<string> AddClassAsync(ClassModel classModel, string token);
        public Task<string> DeleteClassAsync(Guid roomId);
        public Task<(List<Class>, int)> GetAllClassAsync(int pageIndex,int pageSize);
        public Task<(List<Class>, int)> GetClassByIdAsync(string roomId, int pageIndex, int pageSize);
        public Task<string> UpdateClassAsync(Guid roomId, ClassModel classModel, string token);
        public Task<byte[]> ExportToExcelAsync();
    }
}
