using System;
using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class CreateViaggioClass
    {
        public CreateViaggioClass()
        {
        }

        public int CreateViaggio(int IdVersione, string data, double lun, short tempo, int IdM, SqlConnection conn)
        {

            SqlCommand comm = new SqlCommand();
            comm.CommandText = "Matlab.BIS.createViaggioSolSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_versione", IdVersione);
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@km", lun);
            comm.Parameters.AddWithValue("@tempo", tempo);
            comm.Parameters.AddWithValue("@idMezzo", IdM);

            comm.Connection = conn;

            conn.Open();

            comm.ExecuteNonQuery();
            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            DataTable table = new DataTable();
            adapter.Fill(table);

            int IdViaggio = (int)(Int64)table.Rows[0]["id"];
            
            conn.Close();
            return IdViaggio;
        }
    }
}
