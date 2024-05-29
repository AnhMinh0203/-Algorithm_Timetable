using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;
using Dapper;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing.Printing;

namespace TimeTable.Repository
{
    public class ClassRepons : IClassRepons
    {
        private readonly ConnectToSql _connectToSql;
        private readonly IConfiguration _configuration;

        public ClassRepons(ConnectToSql connectToSql, IConfiguration configuration) 
        {
            _connectToSql = connectToSql;
            _configuration = configuration;
        }

        public async Task<string> AddClassAsync(ClassModel classModel, string token)
        {
            string result = null;
            try
            {
                using(var connect = _connectToSql.CreateConnection())
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
                    command.CommandText = "AddClass";
                    Guid IdClass = Guid.NewGuid();
                    command.Parameters.AddWithValue("@Id", IdClass);
                    command.Parameters.AddWithValue("@Name", classModel.NameClass);
                    command.Parameters.AddWithValue("@Year_Of_Admission", classModel.Year_Of_Admission);
                    command.Parameters.AddWithValue("@Course", classModel.Course);
                    command.Parameters.AddWithValue("@Description", classModel.DescriptionClass);
                    command.Parameters.AddWithValue("@CreateBy", UserId);
                    command.Parameters.AddWithValue("@CreateDate", DateTime.UtcNow);
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

        public async Task<string> DeleteClassAsync(Guid roomId)
        {
            string result = string.Empty;
            try
            {
                using(var connect = _connectToSql.CreateConnection())
                {
                    SqlCommand command = new SqlCommand();
                    var command1 = connect.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DeleteClass";
                    command.Parameters.AddWithValue("@Id", roomId);
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

        public async Task<byte[]> ExportToExcelAsync()
        {
            try
            {
                using (var connection = _connectToSql.CreateConnection())
                {
                    List<ReportClass> reportClass = (await connection.QueryAsync<ReportClass>("ReportExcelClass", commandType: CommandType.StoredProcedure)).ToList();
                    byte[] excelBytes = ExportToExcel(reportClass);
                    DateTime dateTime = DateTime.Now;
                    // Lưu dữ liệu vào một tệp Excel tạm thời trên đĩa
                    string tempPath = Path.Combine(Path.GetTempPath(), "Class" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second +".xlsx");
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

        public byte[] ExportToExcel(List<ReportClass> reportClass)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Class");

                // Fill data into the Excel sheet
                worksheet.Cells.LoadFromCollection(reportClass, true);

                // Customize column styles
                using (var range = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.Aqua); // Example color
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
                var dateColumns = new List<int> { 4, 6, 8 }; // Assuming DateStart is column 3 and DateEnd is column 4
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

        public async Task<(List<Class>, int)> GetAllClassAsync(int pageIndex, int pageSize)
        {

            try
            {
                using(var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@NameTable", "Class");
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<Class>(
                        "GetAllClass",
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

        public async Task<(List<Class>, int)> GetClassByIdAsync(string roomId, int pageIndex, int pageSize)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@NameTable", "Class");
                    parameters.Add("@Id", roomId);
                    parameters.Add("@pageIndex", pageIndex);
                    parameters.Add("@pageSize", pageSize);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<Class>(
                        "GetClassById",
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

        public async Task<string> UpdateClassAsync(Guid roomId, ClassModel classModel, string token)
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
                command.CommandText = "UpdateClass";
                command.Parameters.AddWithValue("@Id", roomId);
                command.Parameters.AddWithValue("@Name", classModel.NameClass);
                command.Parameters.AddWithValue("@Year_Of_Admission", classModel.Year_Of_Admission);
                command.Parameters.AddWithValue("@Course", classModel.Course);
                command.Parameters.AddWithValue("@Description", classModel.DescriptionClass);
                command.Parameters.AddWithValue("@ModifiedBy", UserId);
                command.Parameters.AddWithValue("@ModifiedDate", (DateTime.Now));
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
    }
}
