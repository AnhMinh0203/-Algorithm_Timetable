using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ConnectToSql _connectToSql;
        private readonly IConfiguration _configuration;

        public UserRepository(ConnectToSql connectToSql, IConfiguration configuration) 
        {
            _connectToSql = connectToSql;
            _configuration = configuration;
        }
        public async Task<string> SignInAsync(SignInModel signInModel)
        {
            try
            {
                var salt = PasswordManager.GenerateSalt(); // Tạo chuỗi salt mới cho mật khẩu
                var hashedPassword = PasswordManager.HashPassword(signInModel.PassWordHas, salt);
                string query = "select count(*) from dbo.Users where Email = @Email and PassWordHas = @PassWordHas";
                var parameter = new DynamicParameters();
                parameter.Add("Email", signInModel.Email, DbType.String);
                parameter.Add("PassWordHas", hashedPassword, DbType.String);

                using (var connect = _connectToSql.CreateConnection())
                {
                    string roles = null;
                    var count = await connect.ExecuteScalarAsync<int>(query, parameter);
                    if (count != 0)
                    {
                        var userQuery = "SELECT * FROM dbo.Users WHERE Email = @Email";
                        var users = await connect.QueryFirstOrDefaultAsync<Users>(userQuery, parameter);
                        if (users.UsedState == 1)
                        {
                            return "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Admin để mở";
                        }

                        using (var connect1 = _connectToSql.CreateConnection())
                        {
                            SqlCommand command = new SqlCommand();
                            var command1 = connect.CreateCommand();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "GetRolesUser";
                            command.Parameters.AddWithValue("@Email", signInModel.Email);
                            command.Connection = (SqlConnection)connect;

                            // Add the @Result parameter for the stored procedure (output parameter).
                            SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                            resultParam.Direction = ParameterDirection.Output;
                            command.Parameters.Add(resultParam);
                            connect.Open(); // Open the connection before executing the command.
                            await command.ExecuteNonQueryAsync();
                            roles = resultParam.Value.ToString();
                        }
                        var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, signInModel.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim("UserName", users.UserName),
                            new Claim("Email", users.Email),
                            new Claim("UserId", users.Id.ToString()),
                            new Claim(ClaimTypes.Role, roles),
                            new Claim ("PhoneNumber", users.Phone),
                            new Claim ("Gender", users.Gender),
                            new Claim ("DateOfBirth", users.DateOfBirth.ToString()),
                            new Claim("Avata", users.Avata),

                        };
                        var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:ISK"]));
                        var token = new JwtSecurityToken(
                                issuer: _configuration["JWT:ValidIssuer"],
                                audience: _configuration["JWT:ValidAudience"],
                                expires: DateTime.Now.AddMinutes(30),
                                claims: authClaims,
                                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                        );

                        return new JwtSecurityTokenHandler().WriteToken(token);
                    }
                    else
                    {
                        return "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Users> GetByIdAsync(string email)
        {
            using(var connect = _connectToSql.CreateConnection())
            {
                var searchEmail = await connect.QueryFirstOrDefaultAsync<Users>("GetByEmail", new { Email = email }, commandType: CommandType.StoredProcedure);
                return searchEmail;
            }
        }

        public async Task<string> SignUpAsync(SignUpModel signUpModel)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var salt = PasswordManager.GenerateSalt(); // Tạo chuỗi salt mới cho mật khẩu
                    var hashedPassword = PasswordManager.HashPassword(signUpModel.Password, salt); // Mã hóa mật khẩu

                    var parameter = new DynamicParameters();
                    parameter.Add("FirstName", signUpModel.FirstName, DbType.String);
                    parameter.Add("LastName", signUpModel.LastName, DbType.String);
                    parameter.Add("Email", signUpModel.Email, DbType.String);
                    parameter.Add("Password", hashedPassword, DbType.String); // Lưu mật khẩu đã mã hóa
                    parameter.Add("PhoneNumber", signUpModel.PhoneNumber, DbType.Int64);
                    parameter.Add("TypeAccount", signUpModel.TypeAccount, DbType.String);
                    parameter.Add("Gender", signUpModel.Gender, DbType.Int32);
                    parameter.Add("DateOfBirth", signUpModel.DateOfBirth, DbType.Date);
                    parameter.Add("Avata", signUpModel.Avata, DbType.String);

                    var result = await connect.QueryFirstOrDefaultAsync<string>("SignUpUser", parameter, commandType: CommandType.StoredProcedure);

                    if (result == "SignUp Success")
                    {
                        return "SignUp Success";
                    }
                    else
                    {
                        return "SignUp Error";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
