using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectRepons _subjectRepons;

        public SubjectController(ISubjectRepons subjectRepons) 
        {
            _subjectRepons = subjectRepons;
        }

        [HttpPost]
        public async Task<MethodResult> AddSubject (SubjectModel subjectModel, string token)
        {
            var result = await _subjectRepons.AddClassAsync (subjectModel, token);
            if (result == null) return MethodResult.ResultWithError(result, 200, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpDelete]
        public async Task<MethodResult> DeleteSubject (Guid id)
        {
            var result = await _subjectRepons.DeleteClassAsync(id);
            if (result == null) return MethodResult.ResultWithError(result, 200, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpPut]
        public async Task<MethodResult> UpdateSubject(Guid id, SubjectModel subjectModel, string token)
        {
            var result = await _subjectRepons.UpdateClassAsync(id, subjectModel, token);
            if (result == null) return MethodResult.ResultWithError(result, 200, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
        [HttpGet]
        public async Task<MethodResult> GetAllSubject(int pageIndex, int pageSize)
        {
            var result = await _subjectRepons.GetAllClassAsync(pageIndex, pageSize);
            if (result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2); ;
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpGet("Id")]
        public async Task<MethodResult> GetSubjectById(string Id, int pageIndex, int pageSize)
        {
            var result = await _subjectRepons.GetClassByIdAsync(Id, pageIndex, pageSize);
            if (result.Item1 == null) return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpGet("Excel")]
        public async Task<MethodResult> ExportToExcel()
        {
            var result = await _subjectRepons.ExportToExcelAsync();
            if (result == null) return MethodResult.ResultWithError(result, 200, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
    }
}
