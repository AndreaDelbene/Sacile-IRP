using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class RiempiViaggioClass
    {
        public void RiempiViaggio(int IdViaggio, int Pv, int Ordinale, string Kl, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();
            comm.CommandText = "Matlab.BIS.RiempiViaggioSolZero";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_viaggio_pv", IdViaggio);
            comm.Parameters.AddWithValue("@item", Pv);
            comm.Parameters.AddWithValue("@ordSosta", Ordinale);
            comm.Parameters.AddWithValue("@stringone", Kl);
            comm.Connection = conn;

            conn.Open();

            comm.ExecuteReader();
        }
    }
}
