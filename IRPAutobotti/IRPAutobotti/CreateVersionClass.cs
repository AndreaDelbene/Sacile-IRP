using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class CreateVersionClass
    {
        public CreateVersionClass()
        {
        }

        public int CreateVersion(int baseCarico, string login, string data, int IdSetting, int IdRunner, SqlConnection conn)
        {


            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.createVersionSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_base", baseCarico);
            comm.Parameters.AddWithValue("@generatore", login);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@idSettingVariabili", IdSetting);
            comm.Parameters.AddWithValue("@id_runner", IdRunner);
            
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();

           
            int IdVersion = (int)reader["Data"];
            return IdVersion;
        }
    }
}
