using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangerPassWordController : ControllerBase
    {
        private readonly IChangerPasswordrepons _changerPasswordrepons;

        public ChangerPassWordController(IChangerPasswordrepons changerPasswordrepons) 
        {
            _changerPasswordrepons = changerPasswordrepons;
        }

        [HttpPut]
        public async Task<MethodResult> ChangerPassWord(ChangerPasswordModel changerPasswordModel, string token)
        {
            string result1 = "";
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$";
            if (!Regex.IsMatch(changerPasswordModel.NewPassword, passwordPattern) || !Regex.IsMatch(changerPasswordModel.ConfirmNewPassword,passwordPattern))
            {
                result1 = "Mật khẩu phải có ít nhất 8 ký tự, một chữ cái viết hoa, một chữ cái viết thường và một ký tự đặc biệt";
                return MethodResult.ResultWithError(result1,400,"Error",0);
            }
            if (changerPasswordModel.NewPassword != changerPasswordModel.ConfirmNewPassword)
            {
                result1 = "Vui lòng nhập mật khẩu trùng nhau";
                return MethodResult.ResultWithError(result1, 400, "Error", 0);
            }
            var result = await _changerPasswordrepons.ChangerPasswordreponsAsync(changerPasswordModel, token);
            if (result == "Mật khẩu không đúng")
            {
                result1 = "Mật khẩu không đúng";
                return MethodResult.ResultWithError(result1, 400, "Error", 0);
            }
            result1 = "Đổi mật khẩu thành công   ";
            return MethodResult.ResultWithSuccess(result1, 200, "Successfull", 0);
        }
    }
}
