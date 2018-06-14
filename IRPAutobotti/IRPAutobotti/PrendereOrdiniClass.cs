using System.Data.SqlClient;
using System.Linq;
using System.Data;

namespace IRPAutobotti
{

    public struct PrendereOrdiniStruct
    {
        public int[] MioOrdine;
        public int[] pv;
        public int[] ordini;
        public int[] ordiniD;
        public int[] ordiniBD;
        public int[] ordiniB95;
        public int[] ordiniBS;
        public int[] ordiniAlpino;
        public int[] ordiniBluAlpino;
        public int[] ordinipiumeno;
    }
    class PrendereOrdiniClass
    {
        PrendereOrdiniStruct poStruct;
        public PrendereOrdiniClass()
        {
            poStruct = new PrendereOrdiniStruct();
        }
        public PrendereOrdiniStruct PrendereOrdini(int baseCarico, string data, double peso, SqlConnection conn)
        {
            string p = "{call TIP.BIS.getOrdiniGiornalieriPerSacile(" + data + "," + baseCarico + "," + peso + ")}";
            SqlCommand comm = new SqlCommand(p, conn);
            comm.ExecuteNonQuery();

            var tables = new DataTable();
            using (var curs = new SqlDataAdapter(comm))
            {
                curs.Fill(tables);
            }
            DataTable X = tables.DefaultView.ToTable(false, tables.Columns["Data"].ColumnName);
            poStruct.pv = X.AsEnumerable().Select(r => r.Field<int>("codicePv")).ToArray();
            poStruct.ordini = X.AsEnumerable().Select(r => r.Field<int>("QMille")).ToArray();
            poStruct.ordiniD = X.AsEnumerable().Select(r => r.Field<int>("D")).ToArray();
            poStruct.ordiniB95 = X.AsEnumerable().Select(r => r.Field<int>("B95")).ToArray();
            poStruct.ordiniBS = X.AsEnumerable().Select(r => r.Field<int>("BS")).ToArray();
            poStruct.ordiniBD = X.AsEnumerable().Select(r => r.Field<int>("BD")).ToArray();
            poStruct.ordiniAlpino = X.AsEnumerable().Select(r => r.Field<int>("GA")).ToArray();
            poStruct.ordiniBluAlpino = X.AsEnumerable().Select(r => r.Field<int>("GBA")).ToArray();
            poStruct.ordinipiumeno = X.AsEnumerable().Select(r => r.Field<int>("modalit")).ToArray();
            poStruct.MioOrdine= X.AsEnumerable().Select(r => r.Field<int>("IdMioOrdine")).ToArray();


            return poStruct;
        }
    }
}
