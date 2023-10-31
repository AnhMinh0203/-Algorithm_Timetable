using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassRepons _classRepons;

        public ClassController(IClassRepons classRepons)
        {
            _classRepons = classRepons;
        }

        [HttpPost]
        public async Task<MethodResult> AddClass (ClassModel classModel, string token)
        {
            var result = await _classRepons.AddClassAsync(classModel, token);
            if(result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpDelete]
        public async Task<MethodResult> DeleteClass (Guid id)
        {
            var result = await _classRepons.DeleteClassAsync(id);
            if (result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpGet]
        public async Task<MethodResult> GetALlClass(int pageIndex, int pageSize)
        {
            var result = await _classRepons.GetAllClassAsync( pageIndex,  pageSize);
            if (result.Item1 == null)
            {
                return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            }
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpGet("Id")]
        public async Task<MethodResult> GetById (string id, int pageIndex, int pageSize)
        {
            var result = await _classRepons.GetClassByIdAsync(id, pageIndex, pageSize);
            if (result.Item1 == null)
            {
                return MethodResult.ResultWithError(result.Item1, 400, "Error", result.Item2);
            }
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpPut("Id")]
        public async Task<MethodResult> UpdateClass (Guid id, ClassModel classModel, string token)
        {
            var result = await _classRepons.UpdateClassAsync(id, classModel, token);
            if (result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpGet("Excel")]
        public async Task<MethodResult> ExportToExcel()
        {
            var result = await _classRepons.ExportToExcelAsync();
            if (result == null)
            {
                return MethodResult.ResultWithError(result, 400, "Error", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
    }
}
