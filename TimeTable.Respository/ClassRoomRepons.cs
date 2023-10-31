using Dapper;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class ClassRoomRepons : IClassRoomRepons
    {
        private readonly ConnectToSql _connectToSql;
        private readonly IConfiguration _configuration;

        public ClassRoomRepons(ConnectToSql connectToSql, IConfiguration configuration) 
        {
            _connectToSql = connectToSql;
            _configuration = configuration;
        }
        public async Task<string> AddClassRoomAsync(ClassRoomModel classRoomModel, string token)
        {
            string result = string.Empty;
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
                command.CommandText = "AddClassRoom";
                Guid IdClassroom = Guid.NewGuid();
                command.Parameters.AddWithValue("@Id", IdClassroom);
                command.Parameters.AddWithValue("@Name", classRoomModel.Name);
                command.Parameters.AddWithValue("@Description", classRoomModel.Description);
                command.Parameters.AddWithValue("@CreateBy", UserId);
                command.Parameters.AddWithValue("@CreateDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                command.Connection = (SqlConnection)connect;
                try
                {
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = "Thêm phòng học thành công";
                }
                catch (Exception ex)
                {
                    result = "Thêm phòng học không thành công " + ex.Message;
                }
            }
            return result;
        }

        public async Task<string> DeleteClassRoomAsync(Guid roomId)
        {
            string result = string.Empty;
            using (var connect = _connectToSql.CreateConnection())
            {
                SqlCommand command = new SqlCommand();
                var command1 = connect.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "DeleteClassRoom";
                command.Parameters.AddWithValue("@Id", roomId);
                command.Connection = (SqlConnection)connect;

                // Add the @Result parameter for the stored procedure (output parameter).
                SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                resultParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParam);
                try
                {
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public async Task<(List<ClassRooms>, int)> GetAllClassRoomsAsync(int pageIndex, int pageSize)
        {
            try
            {
                using(var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<ClassRooms>(
                        "GetAllClassRoom",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
                } 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<ClassRooms>,int)> GetClassRoomByIdAsync(string roomId, int pageIndex, int pageSize)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Id", roomId);
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<ClassRooms>(
                        "GetClassRoomById",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    int totalRecords = parameters.Get<int>("@totalRecords");
                    return (result.ToList(), totalRecords);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateClassRommAsync(Guid roomId, ClassRoomModel classRoomModel, string token)
        {
            string result = string.Empty;
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
                command.CommandText = "UpdateClassRoom";
                command.Parameters.AddWithValue("@Id", roomId);
                command.Parameters.AddWithValue("@Name", classRoomModel.Name);
                command.Parameters.AddWithValue("@Description", classRoomModel.Description);
                command.Parameters.AddWithValue("@ModifiedBy", UserId);
                command.Parameters.AddWithValue("@ModifiedDate", DateTime.UtcNow);
                command.Connection = (SqlConnection)connect;

                // Add the @Result parameter for the stored procedure (output parameter).
                SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.NVarChar, -1);
                resultParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(resultParam);
                try
                {
                    connect.Open(); // Open the connection before executing the command.
                    await command.ExecuteNonQueryAsync();
                    result = resultParam.Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return result;
        }

        public async Task<byte[]> ExportToExcelAsync()
        {
            try
            {
                using (var connection = _connectToSql.CreateConnection())
                {
                    List<ReportClassRoom> subjects = (await connection.QueryAsync<ReportClassRoom>("ReportExcelClassRoom", commandType: CommandType.StoredProcedure)).ToList();
                    byte[] excelBytes = ExportToExcel(subjects);
                    // Lưu dữ liệu vào một tệp Excel tạm thời trên đĩa
                    DateTime dateTime = DateTime.Now;
                    string tempPath = Path.Combine(Path.GetTempPath(), "Classrooms" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second + ".xlsx");
                    File.WriteAllBytes(tempPath, excelBytes);

                    // Mở tệp Excel và chèn dữ liệu từ tệp Excel tạm thời
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true
                    });
                    // Trả về dữ liệu Excel dưới dạng byte[]
                    return excelBytes;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] ExportToExcel(List<ReportClassRoom> reportSubjects)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("ClassRooms");

                // Fill data into the Excel sheet
                worksheet.Cells.LoadFromCollection(reportSubjects, true);

                // Customize column styles
                using (var range = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Gray); // Example color
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                // Add border around all cells
                for (int row = 1; row <= worksheet.Dimension.Rows; row++)
                {
                    for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                    {
                        using (var cell = worksheet.Cells[row, col])
                        {
                            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);
                        }
                    }
                }
                // Format Date columns
                var dateColumns = new List<int> { 4, 6 }; // Assuming DateStart is column 3 and DateEnd is column 4
                foreach (var column in dateColumns)
                {
                    using (var columnRange = worksheet.Cells[2, column, worksheet.Dimension.Rows, column])
                    {
                        columnRange.Style.Numberformat.Format = "dd-mm-yyyy"; // Customize the date format as needed
                    }
                }


                // Convert the Excel package to a byte array
                byte[] excelData = package.GetAsByteArray();

                return excelData;
            }
        }
    }
}
