using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace IRPAutobotti
{
    class CreateViaggioClass
    {
        public CreateViaggioClass()
        {
        }

        public int CreateViaggio(int IdVersione, string data, double lun, double tempo, int IdM, SqlConnection conn)
        {
            string p = "{call TIP.BIS.createViaggioSolSacile(" + IdVersione.ToString() + ",'" + data + "'," + lun.ToString() + "," +
                tempo.ToString() + "," + IdM.ToString() + ")}";

            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            int[] temp = tables.AsEnumerable().Select(r => r.Field<int>("Data")).ToArray();
            int IdViaggio = temp[2];
            return IdViaggio;
        }
    }
}
