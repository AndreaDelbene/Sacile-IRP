using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{

    class CreateRunnerClass
    {
        public CreateRunnerClass()
        {
        }

        public int CreateRunner(string login,int BaseDiCarico,string data,SqlConnection conn)
        {

            // connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.createRunnerBySolution";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@generatore", login);
            comm.Parameters.AddWithValue("@id_base", BaseDiCarico);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@algo", "sacile");
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();
            
            int IdRunner = (int)reader["Data"];

            reader.Close();
            conn.Close();
            return IdRunner;
        }
    }
}
