using System.Data.SqlClient;
using System.Linq;
using System.Data;

namespace IRPAutobotti
{

    public struct PrendereOrdiniStruct
    {
        public double[] MioOrdine;
        public double[] pv;
        public double[] ordini;
        public double[] ordiniD;
        public double[] ordiniBD;
        public double[] ordiniB95;
        public double[] ordiniBS;
        public double[] ordiniAlpino;
        public double[] ordiniBluAlpino;
        public double[] ordinipiumeno;
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
            //connection
            SqlCommand comm = new SqlCommand();

            comm.CommandText = "Matlab.BIS.getOrdiniGiornalieriPerSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@base", baseCarico);
            comm.Parameters.AddWithValue("@peso", peso);
            comm.Connection = conn;

            System.Console.WriteLine("Apro connessione..");

            SqlDataAdapter adapter = new SqlDataAdapter(comm);
            conn.Open();
            DataTable table = new DataTable();
            adapter.Fill(table);

            var a = (from DataRow r in table.Rows select r["codicePv"]).ToArray();

            poStruct.pv = (from DataRow r in table.Rows select (double)(decimal)r["codicePv"]).ToArray();
            poStruct.ordini = (from DataRow r in table.Rows select (double)(decimal)r["QMille"]).ToArray();
            poStruct.ordiniD = (from DataRow r in table.Rows select (double)(decimal)r["D"]).ToArray();
            poStruct.ordiniB95 = (from DataRow r in table.Rows select (double)(decimal)r["B95"]).ToArray();
            poStruct.ordiniBS = (from DataRow r in table.Rows select (double)(decimal)r["BS"]).ToArray();
            poStruct.ordiniBD = (from DataRow r in table.Rows select (double)(decimal)r["BD"]).ToArray();
            poStruct.ordiniAlpino = (from DataRow r in table.Rows select (double)(decimal)r["GA"]).ToArray();
            poStruct.ordiniBluAlpino = (from DataRow r in table.Rows select (double)(decimal)r["GBA"]).ToArray();
            poStruct.ordinipiumeno = (from DataRow r in table.Rows select (double)(decimal)r["modalit"]).ToArray();
            poStruct.MioOrdine= (from DataRow r in table.Rows select (double)(decimal)r["IdMioOrdine"]).ToArray();

            conn.Close();
            return poStruct;
        }
    }
}
