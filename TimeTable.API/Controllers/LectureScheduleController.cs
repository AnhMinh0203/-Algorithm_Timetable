using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LectureScheduleController : ControllerBase
    {
        private readonly ILectureSchedureRepons _lectureSchedureRepons;

        public LectureScheduleController(ILectureSchedureRepons lectureSchedureRepons) 
        {
            _lectureSchedureRepons = lectureSchedureRepons;
        }
        [HttpGet]
        public async Task<MethodResult> GetAllLectureSchedure(string token, int pageIndex, int pageSize)
        {
            var result = await _lectureSchedureRepons.GetRegisteredCalendarAsync(token, pageIndex, pageSize);
            if(result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpGet("Id")]
        public async Task<MethodResult> GetLectureSchedureById(string token, string search, int pageIndex, int pageSize)
        {
            var result = await _lectureSchedureRepons.GetSchedureByIdReponsAsync(token, search, pageIndex, pageSize);
            if (result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpGet("Registered_Calendar")]
        public async Task<MethodResult> GetRegisteredCalendar(int pageIndex, int pageSize, int check, string Name)
        {
            var result = await _lectureSchedureRepons.GetAllSchedureReponsAsync(pageIndex, pageSize, check, Name);
            if (result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpPost]
        public async Task<MethodResult> UserRegisterEdCalendar (string token, Guid idSchedure, string Course_Code)
        {
            var result = await _lectureSchedureRepons.UserRegisterEdCalendarAsync(token, idSchedure, Course_Code);
            if(result == null) return MethodResult.ResultWithError(result, 400, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull",0);
        }

    }
}
