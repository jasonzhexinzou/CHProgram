using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemEvaluation
{
    class Program
    {
       
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var sql = "Exec p_syncevaluate";

                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                cmd.ExecuteReader(); 
            }
         
        }
    }
}
