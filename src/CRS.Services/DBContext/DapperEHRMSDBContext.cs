using Microsoft.Data.SqlClient;
using System.Data;

namespace CRS.Services.DBContext;

public class DapperEHRMSDBContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public DapperEHRMSDBContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("EHRMSConnectionDB");
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
