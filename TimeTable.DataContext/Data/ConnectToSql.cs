using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TimeTable.DataContext.Data
{
    public class ConnectToSql
    {
        private readonly IConfiguration _configuration;

        public string? ConnectString { get; }

        public ConnectToSql(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectString = _configuration.GetConnectionString("connectString");
        }
        public IDbConnection CreateConnection() => new SqlConnection(ConnectString);
    }
}
