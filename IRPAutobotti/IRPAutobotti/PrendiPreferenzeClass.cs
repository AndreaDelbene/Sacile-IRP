using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRPAutobotti
{

    public struct PrendiPreferenzeStruct
    {
        public DataTable preferenze;
    }
    class PrendiPreferenzeClass
    {
        PrendiPreferenzeStruct ppStruct;
        public PrendiPreferenzeClass()
        {
            ppStruct = new PrendiPreferenzeStruct();
        }
        public PrendiPreferenzeStruct PrendiPreferenze(int baseCarico, SqlConnection conn)
        {
            string p = "{call TIP.BIS.getPreferenzePuntiVendita(" + baseCarico + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();
            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            ppStruct.preferenze = X;
            return ppStruct;
        }
    }
}
