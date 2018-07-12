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
            SqlDataReader reader;
            comm.CommandText = "Matlab.BIS.getOrdiniGiornalieriPerSacile";
            comm.CommandType = CommandType.StoredProcedure;
            comm.Parameters.AddWithValue("@data", data);
            comm.Parameters.AddWithValue("@base", baseCarico);
            comm.Parameters.AddWithValue("@peso", peso);
            comm.Connection = conn;

            conn.Open();

            reader = comm.ExecuteReader();

            poStruct.pv = (from IDataRecord r in reader select (double)r["codicePv"]).ToArray();
            poStruct.ordini = (from IDataRecord r in reader select (double)r["QMille"]).ToArray();
            poStruct.ordiniD = (from IDataRecord r in reader select (double)r["D"]).ToArray();
            poStruct.ordiniB95 = (from IDataRecord r in reader select (double)r["B95"]).ToArray();
            poStruct.ordiniBS = (from IDataRecord r in reader select (double)r["BS"]).ToArray();
            poStruct.ordiniBD = (from IDataRecord r in reader select (double)r["BD"]).ToArray();
            poStruct.ordiniAlpino = (from IDataRecord r in reader select (double)r["GA"]).ToArray();
            poStruct.ordiniBluAlpino = (from IDataRecord r in reader select (double)r["GBA"]).ToArray();
            poStruct.ordinipiumeno = (from IDataRecord r in reader select (double)r["modalit"]).ToArray();
            poStruct.MioOrdine= (from IDataRecord r in reader select (double)r["IdMioOrdine"]).ToArray();


            return poStruct;
        }
    }
}
