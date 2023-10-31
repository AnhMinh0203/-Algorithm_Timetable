using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class EditAccountRepons : IEditAccountRepons
    {
        private readonly ConnectToSql _connectToSql;

        public EditAccountRepons(ConnectToSql connectToSql) 
        {
            _connectToSql = connectToSql;
        }
        public async Task<string> EditInforAccountAsync( EditAccountModel editAccountModel, string token)
        {
            string result = null;
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var decodedToken = tokenHandler.ReadJwtToken(token);
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim == null)
                    {
                        result = "Token không hợp lệ";
                    }
                    Guid UserId = Guid.Parse(userIdClaim.Value);

                    SqlCommand command = new SqlCommand();
                    var command1 = connect.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UPDATEUser";
                    Guid IdClass = Guid.NewGuid();
                    command.Parameters.AddWithValue("@Id", UserId);
                    command.Parameters.AddWithValue("@FirstName", editAccountModel.FirstName);
                    command.Parameters.AddWithValue("@LastName", editAccountModel.LastName);
                    command.Parameters.AddWithValue("@FullName", editAccountModel.FirstName + " " + editAccountModel.LastName);
                    command.Parameters.AddWithValue("@UserName", editAccountModel.FirstName + " " + editAccountModel.LastName);
                    command.Parameters.AddWithValue("@Gender", editAccountModel.Gender);
                    command.Parameters.AddWithValue("@DateOfBirth", editAccountModel.DateOfBirth);
                    command.Parameters.AddWithValue("@Avata", editAccountModel.Avata);
                    command.Parameters.AddWithValue("@Description", editAccountModel.Description);
                    command.Parameters.AddWithValue("@ModifiedBy", UserId);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                    command.Connection = (SqlConnection)connect;

                    // Add the @Result parameter for the stored procedure (output parameter).
                    SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                    resultParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultParam);
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> EditInforAccountManagerAsync(Guid id,EditAccountManagerModel editAccountManagerModel, string token)
        {
            string result = null;
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var decodedToken = tokenHandler.ReadJwtToken(token);
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                    if (userIdClaim == null)
                    {
                        result = "Token không hợp lệ";
                    }
                    Guid UserId = Guid.Parse(userIdClaim.Value);

                    SqlCommand command = new SqlCommand();
                    var command1 = connect.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UPDATEUser";
                    Guid IdClass = Guid.NewGuid();
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FirstName", editAccountManagerModel.FirstName);
                    command.Parameters.AddWithValue("@LastName", editAccountManagerModel.LastName);
                    command.Parameters.AddWithValue("@FullName", editAccountManagerModel.FirstName + " " + editAccountManagerModel.LastName);
                    command.Parameters.AddWithValue("@UserName", editAccountManagerModel.FirstName + " " + editAccountManagerModel.LastName);
                    command.Parameters.AddWithValue("@Gender", editAccountManagerModel.Gender);
                    //command.Parameters.AddWithValue("@Email", editAccountManagerModel.Email);
                    //command.Parameters.AddWithValue("@PhoneNumber", editAccountManagerModel.NumberPhone);
                    command.Parameters.AddWithValue("@DateOfBirth", editAccountManagerModel.DateOfBirth);
                    //command.Parameters.AddWithValue("@UsedStated", editAccountManagerModel.UsedStated);
                    command.Parameters.AddWithValue("@Avata", editAccountManagerModel.Avata);
                    command.Parameters.AddWithValue("@Description", editAccountManagerModel.Description);
                    command.Parameters.AddWithValue("@ModifiedBy", UserId);
                    command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                    command.Connection = (SqlConnection)connect;

                    // Add the @Result parameter for the stored procedure (output parameter).
                    SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                    resultParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultParam);
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
