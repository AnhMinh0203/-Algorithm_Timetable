using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassRoomController : ControllerBase
    {
        private readonly IClassRoomRepons _classRoomRepons;

        public ClassRoomController(IClassRoomRepons classRoomRepons)
        {
            _classRoomRepons = classRoomRepons;
        }

        [HttpPost]
        public async Task<MethodResult> AddClassRoom(ClassRoomModel model, string token)
        {
            var result = await _classRoomRepons.AddClassRoomAsync(model, token);
            if(result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
        [HttpDelete]
        public async Task<MethodResult> DeleteClassRoom (Guid id)
        {
            var result = await _classRoomRepons.DeleteClassRoomAsync(id);
            if (result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpGet]
        public async Task<MethodResult> GetAllClassRoom(int pageIndex, int pageSize)
        {
            var result = await _classRoomRepons.GetAllClassRoomsAsync(pageIndex,pageSize);
            if(result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpPut]
        public async Task<MethodResult> UpdateClassRoom(Guid idroom, ClassRoomModel model, string token)
        {
            var result = await _classRoomRepons.UpdateClassRommAsync(idroom, model, token);
            if( result == null) return MethodResult.ResultWithError(result, 400, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpGet("Id")]
        public async Task<MethodResult> GetClassRoomById(string id, int pageIndex, int pageSize)
        {
            var result = await _classRoomRepons.GetClassRoomByIdAsync(id, pageIndex, pageSize);
            if (result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpGet("Excel")]
        public async Task<MethodResult> ExportToExcel()
        {
            var result = await _classRoomRepons.ExportToExcelAsync();
            if (result == null) return MethodResult.ResultWithError(result, 400, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
    }
}
