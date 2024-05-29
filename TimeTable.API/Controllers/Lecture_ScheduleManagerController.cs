using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Repository;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Lecture_ScheduleManagerController : ControllerBase
    {
        private readonly ILecture_ScheduleManagerRepons _lecture_ScheduleManagerRepons;

        public Lecture_ScheduleManagerController(ILecture_ScheduleManagerRepons lecture_ScheduleManagerRepons) 
        {
            _lecture_ScheduleManagerRepons = lecture_ScheduleManagerRepons;
        }

        [HttpGet]
        public async Task<MethodResult> GetAllLecture_ScheduleManager(int pageIndex, int pageSize)
        {
            var result = await _lecture_ScheduleManagerRepons.GetAllLecture_ScheduleManagerAsync(pageIndex,pageSize);
            if(result.Item1 == null) MethodResult.ResultWithError(result.Item1,400,"Error",result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1,200,"Successfull", result.Item2);
        }
        [HttpGet("Name")]
        public async Task<MethodResult> GetLecture_ScheduleManagerByName (string name, int pageIndex, int pageSize)
        {
            var reuslt = await _lecture_ScheduleManagerRepons.GetLecture_ScheduleManagerByNameAsync(name, pageIndex, pageSize);
            if(reuslt.Item1 == null) MethodResult.ResultWithError(reuslt.Item1, 400, "Error", reuslt.Item2);
            return MethodResult.ResultWithSuccess(reuslt.Item1, 200, "Successfull", reuslt.Item2);
        }

        [HttpPost("Scheduling")]
        public async Task<MethodResult> Scheduling(SchedulingInputModel schedulingInputModel)
        {
            var result = await _lecture_ScheduleManagerRepons.SchedulingAscync(schedulingInputModel);   
            if(result == null) MethodResult.ResultWithError(result, 400, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull");
        }

        [HttpGet("Excel")]
        public async Task<MethodResult> ExportToExcel()
        {
            var result = await _lecture_ScheduleManagerRepons.ExportToExcelAsync();
            if (result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
    }
}
