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
            comm.CommandText = "Matlab.BIS.createRunnerBySolution";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@generatore", login);
            comm.Parameters.AddWithValue("@id_base", BaseDiCarico);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@algo", "sacile");
            comm.Connection = conn;

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            conn.Open();
            DataTable table = new DataTable();
            adapter.Fill(table);

            int IdRunner = (int)(decimal)table.Rows[0]["id_runner"];

            conn.Close();
            return IdRunner;
        }
    }
}
