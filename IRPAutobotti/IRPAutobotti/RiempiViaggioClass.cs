using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    class RiempiViaggioClass
    {
        public void RiempiViaggio(int IdViaggio, int Pv, int Ordinale, string Kl, SqlConnection conn)
        {
            string p = "{call TIP.BIS.RiempiViaggioSolZero(" + IdViaggio + "," + "Pv" + "," + Ordinale + "," + Kl + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
        }
    }
}
