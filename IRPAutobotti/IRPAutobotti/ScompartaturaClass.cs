using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class ScompartaturaClass
    {
        public int Scompartatura(int IdM, string Quantita, double MENOMILLE, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();
            comm.CommandText = "Matlab.BIS.checkScompartiMezzo";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@idMezzo", IdM);
            comm.Parameters.AddWithValue("@stringaProdotti", Quantita);
            comm.Parameters.AddWithValue("@consentiVuoti", 0);
            comm.Parameters.AddWithValue("@sogliaMenoMille", MENOMILLE);
            comm.Parameters.AddWithValue("@completo", 0);
            comm.Connection = conn;

            conn.Open();

            SqlDataReader reader = comm.ExecuteReader();


            string X = reader["Data"].ToString();
            int x;
            try
            {
                x = X.Length;
            }
            catch
            {
                x = -1;
            }

            reader.Close();
            conn.Close();
            return x;
        }
    }
}
