using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{
    class ScompartaturaClass
    {
        public int Scompartatura(int IdM, int Quantita, int MENOMILLE, SqlConnection conn)
        {
            string p = "{call TIP.BIS.checkScompartiMezzo(" + IdM + "," + Quantita + "," + 0 + "," + MENOMILLE + "," + 0 + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();

        }
    }
}
