using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;

namespace TimeTable.Respository.Interfaces
{
    public interface IClassRoomRepons
    {
        public Task<string> AddClassRoomAsync(ClassRoomModel classRoomModel, string token);
        public Task<string> DeleteClassRoomAsync(Guid roomId);
        public Task<(List<ClassRooms>,int)> GetAllClassRoomsAsync(int pageIndex, int pageSize);
        public Task<(List<ClassRooms>, int)> GetClassRoomByIdAsync(string roomId, int pageIndex, int pageSize);
        public Task<string> UpdateClassRommAsync(Guid roomId, ClassRoomModel classRoomModel, string token);
        public Task<byte[]> ExportToExcelAsync();
    }
}
