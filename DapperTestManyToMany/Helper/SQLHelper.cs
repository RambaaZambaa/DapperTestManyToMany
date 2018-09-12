using System.Data;
using System.Data.SqlClient;

namespace DapperTestManyToMany
{
    public class SQLHelper
    {
        public static IDbConnection CreateConnection()
        {
            var connection = new SqlConnection("Data Source=juslap\\sqlexpress;Initial Catalog=JsonTestDB;Integrated Security=True");
            // Properly initialize your connection here.
            return connection;
        }
    }
}
