using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class CreateViaggioNoMezzoClass
    {
        public CreateViaggioNoMezzoClass()
        {
        }

        public int CreateViaggioNoMezzo(int IdVersione, string data, double lun, double tempo, int IdM, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.createViaggioSolSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_versione", IdVersione);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@km", lun);
            comm.Parameters.AddWithValue("@tempo", tempo);
            comm.Parameters.AddWithValue("@idMezzo", IdM);

            comm.Connection = conn;

            conn.Open();
           
            reader = comm.ExecuteReader();
          
            int IdViaggio = (int)reader["Data"];
            return IdViaggio;
        }
    }
}
