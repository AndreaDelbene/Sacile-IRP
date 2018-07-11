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
            //connection
            SqlCommand comm = new SqlCommand();
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.getOrdiniGiornalieriPerSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@base", baseCarico);
            comm.Parameters.AddWithValue("@peso", peso);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();

            poStruct.pv = (from IDataRecord r in reader select (int)r["codicePv"]).ToArray();
            poStruct.ordini = (from IDataRecord r in reader select (int)r["QMille"]).ToArray();
            poStruct.ordiniD = (from IDataRecord r in reader select (int)r["D"]).ToArray();
            poStruct.ordiniB95 = (from IDataRecord r in reader select (int)r["B95"]).ToArray();
            poStruct.ordiniBS = (from IDataRecord r in reader select (int)r["BS"]).ToArray();
            poStruct.ordiniBD = (from IDataRecord r in reader select (int)r["BD"]).ToArray();
            poStruct.ordiniAlpino = (from IDataRecord r in reader select (int)r["GA"]).ToArray();
            poStruct.ordiniBluAlpino = (from IDataRecord r in reader select (int)r["GBA"]).ToArray();
            poStruct.ordinipiumeno = (from IDataRecord r in reader select (int)r["modalit"]).ToArray();
            poStruct.MioOrdine= (from IDataRecord r in reader select (int)r["IdMioOrdine"]).ToArray();


            return poStruct;
        }
    }
}
