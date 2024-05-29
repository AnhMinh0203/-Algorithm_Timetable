using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository accRepo;

        public UsersController(IUserRepository repo)
        {
            accRepo = repo;
        }
        [HttpPost("SignUp")]
        public async Task<Custommessage> SignUp(SignUpModel signUpModel)
        {
            var result1 = new Custommessage();  
            string validatePhone = @"^0\d{9}$";
            string validateEmail = @"^[a-zA-Z0-9._%+-]+@eaut\.edu\.vn$"; 
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$";
            if (!Regex.IsMatch(signUpModel.PhoneNumber, validatePhone) || !Regex.IsMatch(signUpModel.Email,validateEmail))
            {
                result1.status = "Số điện thoại hoặc email không hợp lệ";
                return result1;
            }
            if(!Regex.IsMatch(signUpModel.Password, passwordPattern))
            {
                result1.status = "Mật khẩu phải có ít nhất 8 ký tự, một chữ cái viết hoa, một chữ cái viết thường và một ký tự đặc biệt";
                return result1;
            }
            if(signUpModel.Password != signUpModel.ConfirmPassword)
            {
                result1.status = "Vui lòng điền hai mật khẩu giống nhau";
                return result1;
            }
            var result = await accRepo.SignUpAsync(signUpModel);
            if (result == null) 
            {
                result1.status = "Lỗi";
                return result1;
            }
            result1.status = "Đăng ký thành công";
            result1.token = result;
            return result1;
        }

        [HttpPost("SignIn")]
        public async Task<Custommessage> SignIn(SignInModel signInModel)
        {
            var result = await accRepo.SignInAsync(signInModel);
            var result1 = new Custommessage();
            var user = await accRepo.GetByIdAsync(signInModel.Email);
            if (string.IsNullOrEmpty(result))
            {
                //result1.status = BadRequest().ToString();
                throw new Exception();
                //return string.Empty;
            }
            if(result == "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin để mở" || result == "Tên đăng nhập hoặc mật khẩu không đúng")
            {
                result1.status = "Error";
            }
            else
            {
                result1.status = "Thành công";
            }
            result1.token = result;
            result1.email = user.Email;
            result1.Id = user.Id;
            result1.name = user.FirstName + " " + user.LastName;
            result1.avata = user.Avata;
            result1.first_name = user.FirstName;
            result1.last_name = user.LastName;
            result1.password = user.PassWordHas;
            return result1;
        }

        [HttpGet("Info")] 
        public async Task<IActionResult> GetInformation(string email)
        {
            var result = await accRepo.GetByIdAsync(email);
            if (result == null) return NotFound(result);
            return Ok(result);
        }

    }
}
