using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserManagerRepons _userManagerRepons;

        public UserManagerController(IUserManagerRepons userManagerRepons) 
        {
            _userManagerRepons = userManagerRepons;
        }

        [HttpGet]
        public async Task<MethodResult> GetAllUser()
        {
            var result = await _userManagerRepons.GetAllUserAsync();
            if(result.Item1 == null) return MethodResult.ResultWithSuccess(result.Item1, 400, "NotFound", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2); 
        }

        [HttpGet("Id")]
        public async Task<MethodResult> GetUserById(string id)
        {
            var result = await _userManagerRepons.GetUserByIdAsync(id);
            if (result.Item1 == null) return MethodResult.ResultWithSuccess(result.Item1, 400, "NotFound", result.Item2);
            return MethodResult.ResultWithSuccess(result.Item1, 200, "Successfull", result.Item2);
        }

        [HttpDelete]
        public async Task<MethodResult> DeleteUserById(Guid id)
        {
            var result = await _userManagerRepons.DeleteUserByIdAsync(id);
            if (result == null) return MethodResult.ResultWithSuccess(result, 400, "NotFound", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }

        [HttpPut]
        public async Task<MethodResult> UpdateUsedStated(Guid id, int usedStated)
        {
            var result = await _userManagerRepons.UpdateUsedStatedByIdAsync(id, usedStated);
            if (result == null) return MethodResult.ResultWithSuccess(result, 400, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
        [HttpPut("LoclAccount")]
        public async Task<MethodResult> LockAccount(string TypeAccount, int UsedStated)
        {
            var result = await _userManagerRepons.LockAccount(TypeAccount, UsedStated);
            if (result == null) return MethodResult.ResultWithSuccess(result, 400, "Error", 0);
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0);
        }
    }
}
