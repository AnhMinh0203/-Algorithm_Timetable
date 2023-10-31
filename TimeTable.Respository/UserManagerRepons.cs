using Dapper;
using System.Data;
using TimeTable.DataContext.Data;
using TimeTable.DataContext.Models;
using TimeTable.Respository.Interfaces;

namespace TimeTable.Repository
{
    public class UserManagerRepons : IUserManagerRepons
    {
        private readonly ConnectToSql _connectToSql;

        public UserManagerRepons(ConnectToSql connectToSql) 
        {
            _connectToSql = connectToSql;
        }

        public async Task<string> DeleteUserByIdAsync(Guid id)
        {
            try
            {
                string result = "";
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result1 = await connect.ExecuteAsync("DELETEById", new {  Id = id }, commandType: CommandType.StoredProcedure);
                    if (result1 >= 1) result = "Xóa thành công";
                    else result = "Xóa thất bại";
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(List<UserManagerModel>, int)> GetAllUserAsync()
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@NameTable", "Users");
                    parameters.Add("@pageIndex", 1);
                    parameters.Add("@pageSize", 10);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<UserManagerModel>(
                        "GetAllUser",
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

        public async Task<(List<UserManagerModel>,int)> GetUserByIdAsync(string id)
        {
            try
            {
                using (var connect = _connectToSql.CreateConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@NameTable", "Users");
                    parameters.Add("@Id", id);
                    parameters.Add("@pageIndex", 1);
                    parameters.Add("@pageSize", 10);
                    parameters.Add("@totalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    var result = await connect.QueryAsync<UserManagerModel>(
                        "GetUserById",
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

        public async Task<string> LockAccount(string TypeAccount, int UsedStated)
        {
            try
            {
                string result = "";
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result1 = await connect.ExecuteAsync("LockAccount", new { TypeAccount = TypeAccount, UsedStated = UsedStated }, commandType: CommandType.StoredProcedure);
                    if (result1 >= 1) result = "Thành công";
                    else result = "Xóa thất bại";
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpdateUsedStatedByIdAsync(Guid id, int UsedState)
        {
            try
            {
                string result = "";
                using (var connect = _connectToSql.CreateConnection())
                {
                    var result1 = await connect.ExecuteAsync("UpdateUsedStatedLock", new { Id = id, UsedState = UsedState }, commandType: CommandType.StoredProcedure);
                    if (result1 >= 1) result = "Thành công";
                    else result = "Xóa thất bại";
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
