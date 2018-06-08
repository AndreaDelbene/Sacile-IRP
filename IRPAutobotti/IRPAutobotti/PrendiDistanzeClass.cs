using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{

    public struct PrendiDistanzeStruct
    {
        od_dep_pv,od_pv_dep,od_pv_pv,od_pv_pv_completa,preferenza_pv_pv
    }
    class PrendiDistanzeClass
    {
        public PrendiDistanzeStruct pdStruct;

        public PrendiDistanzeClass()
        {
            pdStruct = new PrendiDistanzeStruct();
        }

        public PrendiDistanzeStruct PrendiDistanze(int baseCarico, string data, int n_ordini, pv, preferenze, SqlConnection conn)
        {
            string p = "{call TIP.BIS._FRA_GetTabDistance(" + baseCarico + "," + data + ")}";
            return pdStruct;
        }
    }
}
