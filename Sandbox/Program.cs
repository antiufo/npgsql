using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var conn = new Npgsql.MultiNpgsqlConnection("HOST=localhost;PORT=5432;DATABASE=postgres;USER ID=postgres;PASSWORD=makaki;TIMEOUT=15;POOLING=True;MINPOOLSIZE=1;MAXPOOLSIZE=4;COMMANDTIMEOUT=20"))
            {
                conn.Open();
                for (int i = 0; i < 20; i++)
                {
                    var tony = conn.CreateCommand();

                    tony.CommandText = "select pg_typeof(@p0)::text";
                    var para = tony.CreateParameter();
                    para.Value = DateTime.UtcNow;
                    para.ParameterName = "p0";
                    tony.Parameters.Add(para);

                    tony.ExecuteNonQuery();
                    Console.WriteLine("k");

                }

                //p.Value = DateTime.UtcNow;
            }
        }
    }
}
