using Microsoft.AspNetCore.Mvc;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EditAccountController : ControllerBase
    {
        private readonly IEditAccountRepons _editAccountRepons;

        public EditAccountController(IEditAccountRepons editAccountRepons) 
        {
            _editAccountRepons = editAccountRepons;
        }
        [HttpPut]
        public async Task<MethodResult> EditAccount( EditAccountModel editAccountModel, string token)
        {
            var result = await _editAccountRepons.EditInforAccountAsync(editAccountModel, token);
            if(result  == null)
            {
                return MethodResult.ResultWithError(result, 400, "NotFound",0 );
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0); ;
        }

        [HttpPut("Manager")]
        public async Task<MethodResult> EditAccountManager(Guid id,EditAccountManagerModel editAccountModel, string token)
        {
            var result = await _editAccountRepons.EditInforAccountManagerAsync(id, editAccountModel, token);
            if (result == null)
            {
                return MethodResult.ResultWithError(result, 400, "NotFound", 0);
            }
            return MethodResult.ResultWithSuccess(result, 200, "Successfull", 0); ;
        }
    }
}
