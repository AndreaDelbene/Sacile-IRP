using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{

    class CreateRunnerClass
    {
        public CreateRunnerClass()
        {
        }

        public int CreateRunner(string login,int BaseDiCarico,string data,SqlConnection conn)
        {
            /*setdbprefs('DataReturnFormat', 'numeric');
            setdbprefs('NullNumberRead', 'NaN');
            setdbprefs('NullStringRead', 'null');*/

            // connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "BIS.createRunnerBySolution";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@generatore", login);
            comm.Parameters.AddWithValue("@id_base", BaseDiCarico);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@algo", "sacile");
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();

            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            int[] temp = tables.AsEnumerable().Select(r => r.Field<int>("Data")).ToArray();
            int IdRunner = temp[2];
            return IdRunner;
        }
    }
}
