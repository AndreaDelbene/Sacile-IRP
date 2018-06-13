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
            //strcat
            string p = "{call TIP.BIS.changeNotification(" + IdRunner.ToString() + "," +
                IdVersione.ToString() + ")}";
            //connection
            SqlCommand comm = new SqlCommand(p,conn);
            comm.ExecuteNonQuery();
            //conn.Close();
        }
    }
}
