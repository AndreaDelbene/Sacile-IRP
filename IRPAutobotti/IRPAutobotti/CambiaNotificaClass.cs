using System.Data;
using System.Data.SqlClient;

namespace IRPAutobotti
{
    class CambiaNotificaClass
    {
        public CambiaNotificaClass()
        {
        }

        public void CambiaNotifica(int IdRunner, int IdVersione, SqlConnection conn)
        {
            SqlCommand comm = new SqlCommand();
            comm.CommandText = "Matlab.BIS.changeNotification";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@id_runner", IdRunner);
            comm.Parameters.AddWithValue("@id_versione", IdVersione);
            comm.Connection = conn;

            conn.Open();

            comm.ExecuteReader();
        }
    }
}
